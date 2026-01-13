using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TYMDLL
{
//需要开发的接口
    public interface TYMCard
    {
        void StopSampling();
        void StartSampling();
        void SetIntegrationTime(int time);
        //string SendMsg(string msg);
        void RegisterImgCallback(EventHandler e);
        void DisConnect();
        void Connect(string host, string remote, int recvPort, int sendPort, int remoteControlPort, int remoteRecallPort);
        void Init(int integrationTime);
        void SetGain(int gain);
        void IgnoreLines(int num);
    }
}
