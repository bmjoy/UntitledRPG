using System;
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
            var list = mBuff.Values.ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                var buff = list[i];
                buff.Tick();
                if (buff.isActive == false)
                    mBuff.Remove(buff.Buff);
            }
        }
        if (mNerf.Count > 0)
        {
            var list = mNerf.Values.ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                var nerf = list[i];
                nerf.Tick();
                if (nerf.isActive == false)
                    mNerf.Remove(nerf.Nerf);
            }
        }
    }

    public void Stop()
    {
        if (mBuff.Count > 0)
        {
            var list = mBuff.Values.ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                var buff = list[i];
                buff.Inactivate();
                if (buff.isActive == false)
                    mBuff.Remove(buff.Buff);
            }
        }

        if (mNerf.Count > 0)
        {
            var list = mNerf.Values.ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                var nerf = list[i];
                nerf.Inactivate();
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
            buff.isActive = true;
            buff.Activate();
        }
    }

    public void AddNerf(TimedNerf nerf)
    {
        if (mNerf.ContainsKey(nerf.Nerf))
        {
            //mNerf[nerf.Nerf].Activate();
        }
        else
        {
            mNerf.Add(nerf.Nerf, nerf);
            nerf.isActive = true;
            nerf.Activate();
        }

    }

    public Buff GetBuff(Type name)
    {
        for (int i = 0; i < mBuff.Count; ++i)
        {
            var b = mBuff.ElementAt(i).Key;
            if (b.GetType() == name || b.name.Contains(name.Name))
                return b;
        }
        return null;
    }
    public Nerf GetNerf(Type name)
    {
        for (int i = 0; i < mNerf.Count; ++i)
        {
            var n = mNerf.ElementAt(i).Key;
            if (n.GetType() == name || n.name.Contains(name.Name))
                return n;
        }
        return null;
    }

    public int GetBuffCount()
    {
        return mBuff.Count;
    }

    public int GetNerfCount()
    {
        return mNerf.Count;
    }
}
