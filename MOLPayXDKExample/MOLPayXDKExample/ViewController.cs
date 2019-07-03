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

			Dictionary<string, object> paymentDetails = new Dictionary<string, object>();
			paymentDetails.Add(MOLPay.mp_amount, "");
			paymentDetails.Add(MOLPay.mp_username, "");
			paymentDetails.Add(MOLPay.mp_password, "");
			paymentDetails.Add(MOLPay.mp_merchant_ID, "");
			paymentDetails.Add(MOLPay.mp_app_name, "");
			paymentDetails.Add(MOLPay.mp_verification_key, "");
			paymentDetails.Add(MOLPay.mp_order_ID, "");
			paymentDetails.Add(MOLPay.mp_currency, "MYR");
			paymentDetails.Add(MOLPay.mp_country, "MY");
			paymentDetails.Add(MOLPay.mp_channel, "");
			paymentDetails.Add(MOLPay.mp_bill_description, "billdesc");
			paymentDetails.Add(MOLPay.mp_bill_name, "billname");
			paymentDetails.Add(MOLPay.mp_bill_email, "example@email.com");
			paymentDetails.Add(MOLPay.mp_bill_mobile, "+60123456789");
			paymentDetails.Add(MOLPay.mp_channel_editing, false);
			paymentDetails.Add(MOLPay.mp_editing_enabled, false);
			//paymentDetails.Add(MOLPay.mp_is_escrow, "");
			//paymentDetails.Add(MOLPay.mp_transaction_id, "");
			//paymentDetails.Add(MOLPay.mp_request_type, "");
			//string[] binlock = new string[] { "", "" };
			//paymentDetails.Add(MOLPay.mp_bin_lock, binlock);
			//paymentDetails.Add(MOLPay.mp_bin_lock_err_msg, "");
			//paymentDetails.Add(MOLPay.mp_custom_css_url, Path.Combine(NSBundle.MainBundle.BundlePath, "Content/custom.css"));
			//paymentDetails.Add(MOLPay.mp_preferred_token, "");
			//paymentDetails.Add(MOLPay.mp_tcctype, "");
			//paymentDetails.Add(MOLPay.mp_is_recurring, false);
			//paymentDetails.Add(MOLPay.mp_sandbox_mode, false);
			//string[] allowedChannels = new string[] { "", "" };
			//paymentDetails.Add(MOLPay.mp_allowed_channels, allowedChannels);
			//paymentDetails.Add(MOLPay.mp_express_mode, false);
			//paymentDetails.Add(MOLPay.mp_advanced_email_validation_enabled, false);
			//paymentDetails.Add(MOLPay.mp_advanced_phone_validation_enabled, false);
			//paymentDetails.Add(MOLPay.mp_bill_name_edit_disabled, true);
			//paymentDetails.Add(MOLPay.mp_bill_email_edit_disabled, true);
			//paymentDetails.Add(MOLPay.mp_bill_mobile_edit_disabled, true);
			//paymentDetails.Add(MOLPay.mp_bill_description_edit_disabled, true);

			MOLPay molpay = new MOLPay(paymentDetails, MolpayCallback);
			molpay.Title = "MOLPayXDK";
			NavigationController.PushViewController(molpay, false);
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
		}

		public void MolpayCallback(string transactionResult)
		{
			Console.WriteLine("MolpayCallback transactionResult = " + transactionResult);
			NavigationController.PopViewController(false);
		}
	}
}