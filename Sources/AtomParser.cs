using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code.Sources
{
    class AtomParser
    {
        int serial, resSeq;
        string recordName, atomName, resName, chainID, elementType;
        Vector3 atomPosition;
        public AtomParser(string lineOfFile)
        {
            float x, y, z;
            x = float.Parse(lineOfFile.Substring(30, 8).Trim());
            y = float.Parse(lineOfFile.Substring(38, 8).Trim());
            z = float.Parse(lineOfFile.Substring(46, 8).Trim());
            atomPosition = new Vector3(x, y, z);
            recordName = lineOfFile.Substring(0, 6).Trim();
            serial = Int32.Parse(lineOfFile.Substring(6, 5).Trim());
            atomName = lineOfFile.Substring(12, 4).Trim();
            resName = lineOfFile.Substring(17, 3).Trim();
            chainID = lineOfFile.Substring(21, 1).Trim();
            resSeq = Int32.Parse(lineOfFile.Substring(22, 4).Trim());
            elementType = lineOfFile.Substring(77, 1).Trim();
        }

        public int GetSerial ()
        {
            return serial;
        }
        public int GetResidueSequence()
        {
            return resSeq;
        }
        public Vector3 GetAtomPosition()
        {
            return atomPosition;
        }
        public string GetRecordName()
        {
            return recordName;
        }
        public string GetAtomName()
        {
            return atomName;
        }
        public string GetResidueName()
        {
            return resName;
        }
        public string GetChainID()
        {
            return chainID;
        }
        public string GetElementType()
        {
            return elementType;
        }
    }
}

