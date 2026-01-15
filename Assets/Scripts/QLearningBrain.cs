using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

[System.Serializable]
public class QEntry
{
    public string state;
    public float[] qValues;
}

[System.Serializable]
public class QDataWrapper
{
    public List<QEntry> entries = new List<QEntry>();
}

public class QLearningBrain
{
    public Dictionary<string, float[]> qTable = new Dictionary<string, float[]>();

    private float learningRate = 0.5f;
    private float discountFactor = 0.9f;
    private int actionCount;        

    public QLearningBrain(int actionCount)
    {
        this.actionCount = actionCount;
    }


    public int GetAction(string state, float epsilon)
    {
    
        if (!qTable.ContainsKey(state))
        {
            qTable[state] = new float[actionCount];
        }


        if (Random.value < epsilon)
        {
            return Random.Range(0, actionCount);
        }
        else
        {
  
            float[] values = qTable[state];
            float maxValue = values.Max();
            return System.Array.IndexOf(values, maxValue);
        }
    }

 
    public void Learn(string state, int action, float reward, string nextState)
    {
        if (!qTable.ContainsKey(nextState))
        {
            qTable[nextState] = new float[actionCount];
        }

        float currentQ = qTable[state][action];
        float maxNextQ = qTable[nextState].Max();


        float newQ = currentQ + learningRate * (reward + (discountFactor * maxNextQ) - currentQ);
        qTable[state][action] = newQ;
    }


    public void SaveBrain(string path)
    {
        QDataWrapper wrapper = new QDataWrapper();
        foreach (var item in qTable)
        {
            wrapper.entries.Add(new QEntry { state = item.Key, qValues = item.Value });
        }
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(path, json);
        Debug.Log("Beyin Kaydedildi: " + path);
    }

    public void LoadBrain(string jsonContent)
    {
        QDataWrapper wrapper = JsonUtility.FromJson<QDataWrapper>(jsonContent);
        qTable.Clear();
        if (wrapper != null && wrapper.entries != null)
        {
            foreach (var item in wrapper.entries)
            {
                qTable[item.state] = item.qValues;
            }
        }
        Debug.Log("Beyin Yüklendi. Öğrenilen Durum Sayısı: " + qTable.Count);
    }
}