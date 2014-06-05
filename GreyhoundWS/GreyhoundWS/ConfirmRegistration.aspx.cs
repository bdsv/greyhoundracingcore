using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using GreyhoundBE.BLL;

namespace GreyhoundWS
{
    public partial class ConfirmRegistration : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int result = 0;

            if ((Request.QueryString["user_id"] != null) & (Request.QueryString["email"] != null))
            {
                String user_id = Request.QueryString["user_id"];
                String email = Request.QueryString["email"];

                result = UserBLL.ConfirmUserRegistration(user_id, email);

            }
            if (result > 0)
            {
                Response.Write("Thanks for the activation.");
            }
            else
            {
                Response.Write("Failed to perform activatation.");
            }
        }
    }
}