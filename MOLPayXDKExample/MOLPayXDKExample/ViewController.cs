using System;
using System.Collections.Generic;
using UIKit;
using System.IO;
using Foundation;
using Newtonsoft.Json;
using MOLPayXDK;

namespace MOLPayXDKExample
{
	public partial class ViewController : UIViewController
	{
		protected ViewController(IntPtr handle) : base(handle)
		{
			
		}

		public ViewController()
		{ 
		
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			Dictionary<String, object> paymentDetails = new Dictionary<String, object>();
			paymentDetails.Add(MOLPay.mp_amount, "");
			paymentDetails.Add(MOLPay.mp_username, "");
			paymentDetails.Add(MOLPay.mp_password, "");
			paymentDetails.Add(MOLPay.mp_merchant_ID, "");
			paymentDetails.Add(MOLPay.mp_app_name, "");
			paymentDetails.Add(MOLPay.mp_verification_key, "");
			paymentDetails.Add(MOLPay.mp_order_ID, "");
			paymentDetails.Add(MOLPay.mp_currency, "");
			paymentDetails.Add(MOLPay.mp_country, "");
			paymentDetails.Add(MOLPay.mp_channel, "");
			paymentDetails.Add(MOLPay.mp_bill_description, "");
			paymentDetails.Add(MOLPay.mp_bill_name, "");
			paymentDetails.Add(MOLPay.mp_bill_email, "");
			paymentDetails.Add(MOLPay.mp_bill_mobile, "");
			paymentDetails.Add(MOLPay.mp_channel_editing, false);
			paymentDetails.Add(MOLPay.mp_editing_enabled, false);
			//paymentDetails.Add(MOLPay.mp_is_escrow, "");
			//paymentDetails.Add(MOLPay.mp_transaction_id, "");
			//paymentDetails.Add(MOLPay.mp_request_type, "");
			//String[] binlock = new String[] { "", "" };
			//paymentDetails.Add(MOLPay.mp_bin_lock, binlock);
			//paymentDetails.Add(MOLPay.mp_bin_lock_err_msg, "");
			//paymentDetails.Add(MOLPay.mp_custom_css_url, Path.Combine(NSBundle.MainBundle.BundlePath, "Content/custom.css"));
			//paymentDetails.Add(MOLPay.mp_preferred_token, "");

			MOLPay molpay = new MOLPay(paymentDetails, MolpayCallback);
			molpay.Title = "MOLPayXDK";
			NavigationController.PushViewController(molpay, true);
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
		}

		public void MolpayCallback(string transactionResult)
		{
			Console.WriteLine("MolpayCallback transactionResult = " + transactionResult);
		}
	}
}