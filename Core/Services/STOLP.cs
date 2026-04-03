using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class STOLP
    {
        public List<TextVector> TextStolp(List<TextVector> data)
        {
            List<TextVector> clearData = new List<TextVector>();

            for(int i = 0; i < data.Count; i++)
            {
                var currentVector = data[i];
                double minFriendDist = double.MaxValue;
                double minEnemyDist = double.MaxValue;

                double epsilon = 0.0001;

                for (int j = 0; j < data.Count; j++)
                {
                    if (i == j) continue;

                    double dist = GetDistance(currentVector, data[j]);

                    if (dist < epsilon) continue;

                    if (currentVector.Label == data[j].Label)
                    {
                        if (dist < minFriendDist) minFriendDist = dist;
                    }
                    else
                    {
                        if(dist < minEnemyDist) minEnemyDist = dist;
                    }                    
                }
                if (minFriendDist == double.MaxValue || minEnemyDist == double.MaxValue) continue;

                double margin = minEnemyDist - minFriendDist;
                if (margin > 0.05)
                {
                    clearData.Add(currentVector);
                }

            }
            return clearData;
        }

        public double GetDistance(TextVector v1, TextVector v2)
        {
            double sum = 0;
            int i = 0, j = 0;

            while (i < v1.Indexes.Length && j < v2.Indexes.Length)
            {
                if (v1.Indexes[i] == v2.Indexes[j])
                {
                    double diff = v1.Weights[i] - v2.Weights[j];
                    sum += diff * diff;
                    i++; j++;
                }
                else if (v1.Indexes[i] < v2.Indexes[j])
                {
                    sum += v1.Weights[i] * v1.Weights[i];
                    i++;
                }
                else
                {
                    sum += v2.Weights[j] * v2.Weights[j];
                    j++;
                }
            }

            while (i < v1.Indexes.Length)
            {
                sum += v1.Weights[i] * v1.Weights[i];
                i++;
            }

            while (j < v2.Indexes.Length)
            {
                sum += v2.Weights[j] * v2.Weights[j];
                j++;
            }

            return Math.Sqrt(sum);
        }
    }
}
