using Microsoft.Extensions.Options;
using PracticeAuthorizationAndAuthentication.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PracticeAuthorizationAndAuthentication.DataMapper
{
    public class TokenDataMapper : ITokenDataMapper
    {
        private readonly ConnectionStringSetting _conStr;

        public TokenDataMapper(IOptions<ConnectionStringSetting> conStr)
        {
            _conStr = conStr.Value;
        }

        public string DeleteClaim(string name)
        {
            string deletedClaimValue;
            using (var con = new SqlConnection(_conStr.DefaultConnection))
            {
                var cmd = new SqlCommand("DeleteClaims", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@name", name);

                con.Open();

                var result = cmd.ExecuteScalar();
                deletedClaimValue = result.ToString();
            }
            return deletedClaimValue;
        }

        public List<ClaimModel> GetAllClaims()
        {
            var response = new List<ClaimModel>();
            using (var con = new SqlConnection(_conStr.DefaultConnection))
            {
                var cmd = new SqlCommand("GetAllClaims", con);
                cmd.CommandType = CommandType.StoredProcedure;

                var ds = new DataSet();

                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(ds);
                }

                var tab = ds.Tables[0];
                foreach (DataRow r in tab.Rows)
                {
                    var c = new ClaimModel();
                    c.Name = r["name"].ToString();
                    c.value = r["value"].ToString();
                    response.Add(c);
                }
            }

            return response;
        }

        public ClaimModel InsertClaim(string name, string value)
        {
            var model = new ClaimModel();
            using (var con = new SqlConnection(_conStr.DefaultConnection))
            {
                var cmd = new SqlCommand("AddClaims", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@value", value);

                var ds = new DataSet();

                using (var adp = new SqlDataAdapter(cmd))
                {
                    adp.Fill(ds);
                }

                var claims = ds.Tables[0];

                foreach (DataRow dr in claims.Rows)
                {
                    var cname = dr["name"].ToString();
                    var cval = dr["value"].ToString();
                    model.Name = cname;
                    model.value = cval;
                }
            }
            return model;
        }

        public List<ClaimModel> InsertClaims(List<ClaimModel> claims)
        {
            var responseClaims = new List<ClaimModel>();
            using (var con = new SqlConnection(_conStr.DefaultConnection))
            {
                var cmd = new SqlCommand("InsertClaims", con);
                cmd.CommandType = CommandType.StoredProcedure;
                var param = new SqlParameter("@claims", GetDataTable(claims));
                cmd.Parameters.Add(param);
                var ds = new DataSet();
                using (var adp = new SqlDataAdapter(cmd))
                {
                    adp.Fill(ds);
                }

                var resultTable = ds.Tables[0];

                foreach (DataRow row in resultTable.Rows)
                {
                    var claim = new ClaimModel();
                    claim.Name = row["name"].ToString();
                    claim.value = row["value"].ToString();
                    responseClaims.Add(claim);
                }
            }

            return responseClaims;
        }

        public DataTable GetDataTable(List<ClaimModel> claims)
        {
            var table = new DataTable();
            table.Columns.Add("name");
            table.Columns.Add("value");

            foreach (var claim in claims)
            {
                table.Rows.Add(claim.Name, claim.value);
            }

            return table;
        }
    }
}