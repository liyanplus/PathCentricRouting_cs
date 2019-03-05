using System;
using System.Collections.Generic;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Distributions;

namespace EnergyCostEstimator
{
    public class Scenario
    {
        #region property
        public bool SingleVehicleModel { get; set; }

        public Matrix<double> VFactors { get; set; }

        public Vector<double> SignatureCostMean { get; set; }
        public Matrix<double> SignatureCostVar { get; set; }
        #endregion
        public Scenario()
        {
        }

        public Scenario(TraceDB ptraces, List<uint> ppassingTraceIds, List<int> pstartingTracePartIdxes, int ppathLen,
            bool psingleModel = true)
        {
            if (psingleModel)
            {
                // in trace database, there is only one vehicle model
                SingleVehicleModel = psingleModel;

                // initialize the data
                Matrix<double> costMat = Matrix<double>.Build.Dense(ppassingTraceIds.Count, ppathLen);

                for (int traceIdx = 0; traceIdx < ppassingTraceIds.Count; traceIdx++)
                {
                    for (int tracePartIdx = 0; tracePartIdx < ppathLen; tracePartIdx++)
                    {
                        costMat[traceIdx, tracePartIdx] = ptraces[ppassingTraceIds[traceIdx]].Costs
                            [pstartingTracePartIdxes[tracePartIdx] + tracePartIdx];
                    }
                }

                SignatureCostMean = costMat.ColumnSums().Divide(costMat.RowCount);

                var centralizedCostMat = costMat -
                    Vector<double>.Build.Dense(ppassingTraceIds.Count, 1).OuterProduct(SignatureCostMean);

                SignatureCostVar = centralizedCostMat.TransposeThisAndMultiply(centralizedCostMat);
            }
            else
            {
                // initialize the data
                Matrix<double> costMat = Matrix<double>.Build.Dense(ppassingTraceIds.Count, ppathLen);
                Matrix<double> paraMat = Matrix<double>.Build.Dense(ppassingTraceIds.Count, 2);


                for (int traceIdx = 0; traceIdx < ppassingTraceIds.Count; traceIdx++)
                {
                    paraMat[traceIdx, 0] = ptraces[ppassingTraceIds[traceIdx]].Paras.MassFactor;
                    paraMat[traceIdx, 1] = ptraces[ppassingTraceIds[traceIdx]].Paras.AreaFactor;

                    for (int tracePartIdx = 0; tracePartIdx < ppathLen; tracePartIdx++)
                    {
                        costMat[traceIdx, tracePartIdx] = ptraces[ppassingTraceIds[traceIdx]].Costs
                            [pstartingTracePartIdxes[tracePartIdx] + tracePartIdx];
                    }
                }

                // estimate the vlinear and vcube factors
                if (Math.Abs(paraMat.Column(0).Maximum() - paraMat.Column(0).Minimum()) < 1e-3 &&
                    Math.Abs(paraMat.Column(1).Maximum() - paraMat.Column(1).Minimum()) < 1e-3)
                {
                    // if in this scenario there is only one vehicle model in the trajectory data in the scenario
                    // vlinear and vcube cannot be estimated
                    SingleVehicleModel = true;
                }
                else
                {
                    SingleVehicleModel = false;

                    VFactors = paraMat.TransposeThisAndMultiply(paraMat).PseudoInverse().TransposeAndMultiply(paraMat).Multiply(costMat);

                    var approxCostMat = paraMat.Multiply(paraMat);

                    var centralizedCostMat = costMat - approxCostMat;

                    SignatureCostVar = centralizedCostMat.TransposeThisAndMultiply(centralizedCostMat);
                }
            }
        }

        public double TraceInScenarioLikelihood(Trace ptrace, int pstartingTracePartIdxes)
        {
            Vector<double> traceCost = ptrace.Costs.SubVector(pstartingTracePartIdxes, SignatureCostMean.Count);

            Matrix<double> costMeanMat = null;

            if (SingleVehicleModel)
            {
                // if the trajectory database only contains one vehicle model
                // compare the input trajectory cost with the signature mean cost

                costMeanMat = SignatureCostMean.ToRowMatrix();
            }
            else
            {
                // if the trajectory database has more than one vehicle model
                // compare the input trajectory cost with the calculated expected cost

                var paraMat = Matrix<double>.Build.Dense(1, 2);
                paraMat[0, 0] = ptrace.Paras.MassFactor;
                paraMat[0, 1] = ptrace.Paras.AreaFactor;

                costMeanMat = paraMat.Multiply(VFactors);
            }

            var costDistribution = new MatrixNormal(
                    costMeanMat,
                    Matrix<double>.Build.Dense(1, 1, 0),
                    SignatureCostVar);

            return costDistribution.Density(traceCost.ToRowMatrix());
        }

        public double ScenarioTransmissionLikelihood(Scenario potherScenario, int poverlapLen,
            string pdistanceMetric = "KL")
        {
            Matrix<double> thisCostMeanMat = null;
            Matrix<double> otherCostMeanMat = null;
            if (SingleVehicleModel)
            {
                thisCostMeanMat = SignatureCostMean.
                    SubVector(SignatureCostMean.Count - poverlapLen, poverlapLen).
                    ToRowMatrix();
                otherCostMeanMat = potherScenario.
                    SignatureCostMean.SubVector(0, poverlapLen).
                    ToRowMatrix();
            }
            else
            {
                thisCostMeanMat = VFactors.SubMatrix(0, 2, VFactors.ColumnCount - poverlapLen, poverlapLen)
                    .ColumnSums().ToRowMatrix();
                otherCostMeanMat = potherScenario.VFactors.SubMatrix(0, 2, 0, poverlapLen).ColumnSums().ToRowMatrix();
            }

            var thisCostVarMat = SignatureCostVar.SubMatrix(SignatureCostMean.Count - poverlapLen, poverlapLen,
                SignatureCostMean.Count - poverlapLen, poverlapLen);
            var otherCostVarMat = potherScenario.SignatureCostVar.SubMatrix(0, poverlapLen,
                0, poverlapLen);

            if (pdistanceMetric == "KL")
            {
                // use (1 - sigmoid(KL divergence)) * 2 to model the likelihood of the transition
                var kl = (thisCostVarMat.Inverse().Multiply(otherCostVarMat).Trace() +
                    (thisCostMeanMat - otherCostMeanMat).Multiply(thisCostVarMat.Inverse()).TransposeAndMultiply(thisCostMeanMat - otherCostMeanMat).At(0, 0) +
                    Math.Log(thisCostVarMat.Determinant() / otherCostVarMat.Determinant()) - thisCostVarMat.RowCount) / 2;
                return (1 - SpecialFunctions.Logistic(kl)) * 2;
            }
            else if (pdistanceMetric == "Bhattacharyya")
            {
                // use (1 - sigmoid(Bhattacharyya distance)) * 2 to model the likelihood of the transition
                var avgCostVarMat = thisCostVarMat.Add(otherCostVarMat).Divide(2);
                var bd = 1.0 / 8.0 * ((thisCostMeanMat - otherCostMeanMat).Multiply(avgCostVarMat.Inverse()).TransposeAndMultiply(thisCostMeanMat - otherCostMeanMat)).At(0, 0) +
                    0.5 * Math.Log(avgCostVarMat.Determinant() / Math.Sqrt(thisCostVarMat.Determinant() * otherCostVarMat.Determinant()));
                return (1 - SpecialFunctions.Logistic(bd)) * 2;
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}
