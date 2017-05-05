﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using DesignPattern.Objectpool;




namespace DesignPattern.Objectpool
{
    // The PooledObject class is the type that is expensive or slow to instantiate,
    // or that has limited availability, so is to be held in the object pool.


    // The Pool class is the most important class in the object pool design pattern. It controls access to the
    // pooled objects, maintaining a list of available objects and a collection of objects that have already been
    // requested from the pool and are still in use. The pool also ensures that objects that have been released
    // are returned to a suitable state, ready for the next time they are requested. 
    public static class Pool
    {
        private static List<Corps> _available = new List<Corps>();
        private static List<Corps> _inUse = new List<Corps>();

        public static Corps GetObject(PopUnit origin, int size)
        {
            lock (_available)
            {
                if (_available.Count != 0)
                {
                    Corps po = _available[0];
                    po.initialize(origin, size);
                    _inUse.Add(po);
                    _available.RemoveAt(0);
                    return po;
                }
                else
                {
                    Corps po = new Corps(origin, size);
                    _inUse.Add(po);
                    return po;
                }
            }
        }

        public static void ReleaseObject(Corps po)
        {
            //CleanUp(po);
            po.deleteData();

            lock (_available)
            {
                _available.Add(po);
                _inUse.Remove(po);
            }
        }
        public static System.Collections.IEnumerable existing()
        //public System.Collections.IEnumerator GetEnumerator()
        {
            foreach (Corps f in _inUse)
                yield return f;
            //_inUse.ForEach(x=> { yield return x; });            
        }
    }

}
public class Corps
{ 
    PopUnit origin;
    int size;
    //static List<Corps> allCorps = new List<Corps>();

    internal void initialize(PopUnit origin, int size)
    {
        this.origin = origin;
        this.size = size;
    }

    public Corps(PopUnit origin, int size)
    {
        initialize(origin, size);

        //allCorps.Add(this);
    }
    public PopType getType()
    { return origin.type; }
    public int getSize()
    {
        return size;
    }
    override public string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(getSize()).Append(" ").Append(origin.ToString());
        return sb.ToString();
    }

    internal float getStrenght()
    {
        return getSize() * origin.type.getStrenght();
    }

    internal void TakeLoss(int loss)
    {

        int sum = size - loss;
        if (sum > 0)
            size = sum;
        else
            size = 0;
        origin.takeLoss(loss);

    }

    internal PopUnit getPopUnit()
    {
        return origin;
    }

    internal void add(int v)
    {
        size += v;
    }

    internal void demobilize()
    {
        origin.demobilize();
        Pool.ReleaseObject(this);
        
    }
    //Army army;
    internal void deleteData()
    {
        size = 0;
        origin = null;
        //here - delete all links on that object
        //army.demobilize(this);        
    }

   
}