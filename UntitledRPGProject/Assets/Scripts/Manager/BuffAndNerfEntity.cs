using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffAndNerfEntity : MonoBehaviour
{
    private readonly Dictionary<Buff, TimedBuff> mBuff = new Dictionary<Buff, TimedBuff>();
    private readonly Dictionary<Nerf, TimedNerf> mNerf = new Dictionary<Nerf, TimedNerf>();
    public void Tick()
    {
        if (mBuff.Count > 0)
        {
            foreach(var buff in mBuff.Values.ToList())
            {
                buff.Tick();
                if(buff.isActive == false)
                {
                    mBuff.Remove(buff.Buff);
                }
            }
        }
        if (mNerf.Count > 0)
        {
            foreach (var nerf in mNerf.Values.ToList())
            {
                nerf.Tick();
                if (nerf.isActive == false)
                {
                    mNerf.Remove(nerf.Nerf);
                }
            }
        }
    }

    public void AddBuff(TimedBuff buff)
    {
        if(mBuff.ContainsKey(buff.Buff))
        {
            mBuff[buff.Buff].Activate();
        }
        else
        {
            mBuff.Add(buff.Buff, buff);
            buff.Activate();
        }
    }

    public void AddNerf(TimedNerf nerf)
    {
        if (mNerf.ContainsKey(nerf.Nerf))
        {
            mNerf[nerf.Nerf].Activate();
        }
        else
        {
            mNerf.Add(nerf.Nerf, nerf);
            nerf.Activate();
        }
    }
}
