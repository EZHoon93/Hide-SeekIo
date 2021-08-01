using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class ExcelTest : MonoBehaviour
{
    public TextAsset txt;
    string[,] Sentence;
    int lineSize, rowSize;

    void Start()
    {
        // 엔터단위와 탭으로 나눠서 배열의 크기 조정

        string currentText = txt.text.Substring(0, txt.text.Length - 1);
        string[] line = currentText.Split('\n');
        for(int i = 0; i < line.Length; i++)
        {
            string[] worlds = line[i].Split('\t');
            List<double> ss = new List<double>();
            for(int j = 0; j < worlds.Length; j++)
            {
                if (string.IsNullOrEmpty(worlds[j])) continue;
                var newWorld = worlds[j].Replace(",", "");
                int r = 0;
                if (i % 5 == 0)
                {
                    var s = int.TryParse(newWorld, out r);
                    if (s)
                    {
                        if(r > 100)
                        {
                            ss.Add((double)r);
                        }
                    }
                }


            }


            if (i % 5 == 0)
            {
                print(ss.Count);
                print(getStandardDeviation(ss));
            }

        }


    }

    private double getStandardDeviation(List<double> doubleList)
    {
        double average = doubleList.Average();
        double sumOfDerivation = 0;
        foreach (double value in doubleList)
        {
            sumOfDerivation += (value) * (value);
        }
        double sumOfDerivationAverage = sumOfDerivation / doubleList.Count;
        return Math.Sqrt(sumOfDerivationAverage - (average * average));
    }
}
