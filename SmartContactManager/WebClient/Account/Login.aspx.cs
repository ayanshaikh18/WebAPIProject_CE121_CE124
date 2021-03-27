﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebClient.Models;
using WebClient.Models.ViewModels;

namespace WebClient.Account
{
    public partial class Login : System.Web.UI.Page
    {
        static HttpClient client = new HttpClient();
        protected void Page_Load(object sender, EventArgs e)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    this.Context.Items.Add("ErrorMessage", "Access Denied! Please Login");
                    Response.Redirect("~/Dashboard.aspx");
                }
                string Success_Message = (string)this.Context.Items["SuccessMessage"];
                string Error_Message = (string)this.Context.Items["ErrorMessage"];
                if (Success_Message != null)
                {
                    SuccessMessage.Visible = true;
                    SuccessMessage.Text = Success_Message;
                    this.Context.Items.Remove("SuccessMessage");
                    ErrorMessage.Visible = false;
                }
                if (Error_Message != null)
                {
                    ErrorMessage.Visible = true;
                    ErrorMessage.Text = Error_Message;
                    this.Context.Items.Remove("ErrorMessage");
                    SuccessMessage.Visible = false;
                }
            }
        }

        protected async void SubmitButton_Click1(object sender, EventArgs e)
        {
            LoginUser loginUser = new LoginUser();
            loginUser.Email = Email.Text;
            loginUser.Password = Password.Text;

            var serializeduser = JsonConvert.SerializeObject(loginUser);
            var content = new StringContent(serializeduser, Encoding.UTF8, "application/json");
            var result = await client.PostAsync("https://localhost:44373/api/account/login", content);
            RootObject response = JsonConvert.DeserializeObject<RootObject>(await result.Content.ReadAsStringAsync());

            //model state validation remaining
            if (response.isSuccess)
            {
                User user = (User)response.data;
                Session["UserID"] = user.Id;
                Response.Redirect("~/Dashboard.aspx");
            }
            else
            {
                SuccessMessage.Visible = false;
                ErrorMessage.Visible = true;
                ErrorMessage.Text = response.message;
                return;
            }   
        }
    }
}