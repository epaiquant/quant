using EPI.CSharp.Commons;
using EPI.CSharp.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPI.CSharp.Services
{
    public class UserService : BaseService
    {
        public Users Login(string userName, string userPwd)
        {
            var url = HostUrl + "/api/user/login";
            var jsonResult = Net.PostWebRequest(url, string.Format("userName={0}&userPwd={1}", userName, userPwd), Encoding.UTF8);
            ResData  resData = JsonConvert.DeserializeObject<ResData>(jsonResult);
            if (resData.status)
            {
                return JsonConvert.DeserializeObject<Users>(resData.result.ToString());
            }
            else return null;
        }

        public Users GetUserInfo(int userId, string token)
        {
            var url = HostUrl + "/api/user/info?"+ string.Format("id={0}&access_token={1}", userId, token);
            var jsonResult = Net.GetWebRequest(url);
            ResData resData = JsonConvert.DeserializeObject<ResData>(jsonResult);
            if (resData.status)
            {
                return JsonConvert.DeserializeObject<Users>(resData.result.ToString());
            }
            else return null;
        }
    }
}
