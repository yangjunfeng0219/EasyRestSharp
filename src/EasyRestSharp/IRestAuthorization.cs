namespace EasyRestSharp;

using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IRestAuthorization
{
    void Authorize(RestRequest request);
}
