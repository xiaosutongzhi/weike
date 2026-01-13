using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TYMCARD
{
    //需要开发的接口
    public interface TYMCard
    {
        void StopSampling();
        void StartSampling();
        void SetIntegrationTime(int time);
        void RegisterImgCallback(EventHandler e);
        void DisConnect();
        void Connect(string host, string remote, int recvPort, int sendPort, int remoteControlPort, int remoteRecallPort);
        void InitSample(int integrationTime, uint target = 50000, bool correct_flag = true, bool background = true);

        void SetGain(int gain);
        void IgnoreLines(int num);
    }
}
