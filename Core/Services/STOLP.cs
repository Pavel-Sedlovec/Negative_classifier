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

                    double dist = GetDistance(currentVector.Features, data[j].Features);

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

        public double GetDistance(double[] v1, double[] v2)
        {
            double sum = 0;

            for(int i = 0; i<v1.Length; i++)
            {
                double difference = v1[i] - v2[i];
                sum += difference * difference;
            }
            return Math.Sqrt(sum);
        }
    }
}
