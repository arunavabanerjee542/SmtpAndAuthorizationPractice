using PracticeAuthorizationAndAuthentication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PracticeAuthorizationAndAuthentication.DataMapper
{
    public interface ITokenDataMapper
    {
        ClaimModel InsertClaim(string name, string value);

        string DeleteClaim(string name);

        List<ClaimModel> GetAllClaims();

        List<ClaimModel> InsertClaims(List<ClaimModel> claims);
    }
}