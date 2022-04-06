using System.Net;
using System.Text;
using ADbid.subWindows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ADbid.classes
{
    class Deploy
    {
        private string checkUri;
        private string downUri;

        public Deploy()
        {

            checkUri = "http://adbid.co.kr/api/version";
            downUri = "http://adbid.co.kr/common/setupDownload";
        }

        // 버전 정보가 0.0.0.0 식으로 반환 됨
        public string checkVersion()
        {
            try
            {
                var cli = new WebClient();
                cli.Headers[HttpRequestHeader.ContentType] = "application/json";
                string response = cli.DownloadString(checkUri);
                return response;
            }
            catch
            {
                WinMessage popUp = new WinMessage("ADBid 서버와의 연결이 원활하지 않습니다.");
                return null;
            }

        }

        // setup 파일 다운로드 및 자동 실행
        public void download() {

            try
            {
                System.Diagnostics.Process.Start(downUri);
                return;
            }
            catch
            {
                WinMessage popUp = new WinMessage("ADBid 서버와의 연결이 원활하지 않습니다.");
                return;
            }


        }

    }
}
