using System;
using System.Collections.Generic;
using System.IO;
using UIKit;
using Foundation;
using Newtonsoft.Json;

namespace MOLPayXDK
{
	public class MOLPay : UIViewController
	{
		public const string mp_amount = "mp_amount";
		public const string mp_username = "mp_username";
		public const string mp_password = "mp_password";
		public const string mp_merchant_ID = "mp_merchant_ID";
		public const string mp_app_name = "mp_app_name";
		public const string mp_order_ID = "mp_order_ID";
		public const string mp_currency = "mp_currency";
		public const string mp_country = "mp_country";
		public const string mp_verification_key = "mp_verification_key";
		public const string mp_channel = "mp_channel";
		public const string mp_bill_description = "mp_bill_description";
		public const string mp_bill_name = "mp_bill_name";
		public const string mp_bill_email = "mp_bill_email";
		public const string mp_bill_mobile = "mp_bill_mobile";
		public const string mp_channel_editing = "mp_channel_editing";
		public const string mp_editing_enabled = "mp_editing_enabled";
		public const string mp_transaction_id = "mp_transaction_id";
		public const string mp_request_type = "mp_request_type";
		public const string mp_is_escrow = "mp_is_escrow";
		public const string mp_bin_lock = "mp_bin_lock";
		public const string mp_bin_lock_err_msg = "mp_bin_lock_err_msg";
		public const string mp_custom_css_url = "mp_custom_css_url";
		public const string mp_preferred_token = "mp_preferred_token";
		public const string mp_tcctype = "mp_tcctype";
		public const string mp_is_recurring = "mp_is_recurring";
		public const string mp_sandbox_mode = "mp_sandbox_mode";
		public const string mp_allowed_channels = "mp_allowed_channels";
		public const string mp_express_mode = "mp_express_mode";
		public const string mp_advanced_email_validation_enabled = "mp_advanced_email_validation_enabled";
		public const string mp_advanced_phone_validation_enabled = "mp_advanced_phone_validation_enabled";
		public const string mp_bill_name_edit_disabled = "mp_bill_name_edit_disabled";
		public const string mp_bill_email_edit_disabled = "mp_bill_email_edit_disabled";
		public const string mp_bill_mobile_edit_disabled = "mp_bill_mobile_edit_disabled";
		public const string mp_bill_description_edit_disabled = "mp_bill_description_edit_disabled";
		public const string mp_language = "mp_language";
		public const string mp_dev_mode = "mp_dev_mode";
		public const string mp_cash_waittime = "mp_cash_waittime";
		public const string mp_non_3DS = "mp_non_3DS";
		public const string mp_card_list_disabled = "mp_card_list_disabled";
		public const string mp_disabled_channels = "mp_disabled_channels";

		private const string mpopenmolpaywindow = "mpopenmolpaywindow://";
		private const string mpcloseallwindows = "mpcloseallwindows://";
		private const string mptransactionresults = "mptransactionresults://";
		private const string mprunscriptonpopup = "mprunscriptonpopup://";
		private const string mppinstructioncapture = "mppinstructioncapture://";
		private const string molpayresulturl = "MOLPay/result.php";
		private const string molpaynbepayurl = "MOLPay/nbepay.php";
		private const string module_id = "module_id";
		private const string wrapper_version = "wrapper_version";
		private string finishLoadUrl;
		private bool isClosingReceipt = false;
		private bool hijackWindowOpen = false;
		private Action<string> callback;
		private string json;
		private string transactionResults;
		private UIWebView mpMainUI, mpMOLPayUI, mpBankUI;

		public MOLPay(Dictionary<string, object> paymentDetails, Action<string> callback)
		{
			paymentDetails.Add(module_id, "molpay-mobile-xdk-xamarin-ios");
			paymentDetails.Add(wrapper_version, "0");
			this.callback = callback;
			json = JsonConvert.SerializeObject(paymentDetails);
		}

		private void CloseMolpay(object sender, EventArgs e)
		{ 
			mpMainUI.EvaluateJavascript("closemolpay()");
			if (isClosingReceipt)
			{
				isClosingReceipt = false;
				Finish();
			}
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			NavigationItem.SetHidesBackButton(true, false);
			var closeBtn = new UIBarButtonItem("Close", UIBarButtonItemStyle.Plain, CloseMolpay);
			NavigationItem.SetRightBarButtonItem(closeBtn, true);
			NavigationController.NavigationBar.Translucent = false;

			mpMainUI = new UIWebView(View.Bounds);
			View.AddSubview(mpMainUI);
			mpMainUI.ScalesPageToFit = false;
			mpMainUI.ScrollView.Bounces = false;
			mpMainUI.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;

			mpMOLPayUI = new UIWebView(View.Bounds);
			View.AddSubview(mpMOLPayUI);
			mpMOLPayUI.ScalesPageToFit = false;
			mpMOLPayUI.ScrollView.Bounces = false;
			mpMOLPayUI.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			mpMOLPayUI.Hidden = true;

			string fileName = "molpay-mobile-xdk-www/index.html";
			string localHtmlUrl = Path.Combine(NSBundle.MainBundle.BundlePath, fileName);
			mpMainUI.LoadRequest(new NSUrlRequest(new NSUrl(localHtmlUrl, false)));
			mpMainUI.LoadFinished += MPMainUILoadFinished;
			mpMainUI.ShouldStartLoad = MPMainUIShouldStartLoad;
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
		}

		private bool MPMainUIShouldStartLoad(UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
		{
			Console.WriteLine("MPMainUIShouldStartLoad url = " + request.Url);

			if (request.Url != null && request.Url.ToString().StartsWith(mpopenmolpaywindow))
			{
				string base64String = request.Url.ToString().Replace(mpopenmolpaywindow, "");
				base64String = base64String.Replace("-", "+");
				base64String = base64String.Replace("_", "=");
				Console.WriteLine("MPMainUI mpopenmolpaywindow base64String = " + base64String);

				string dataString = Base64Decode(base64String);
				Console.WriteLine("MPMainUI mpopenmolpaywindow dataString = " + dataString);

				if (dataString.Length > 0)
				{
					Console.WriteLine("MPMainUI mpopenmolpaywindow success");
					mpMOLPayUI.LoadHtmlString(dataString, null);
					mpMOLPayUI.Hidden = false;
					mpMOLPayUI.LoadFinished += MPMOLPayUILoadFinished;
					mpMOLPayUI.ShouldStartLoad = MPMOLPayUIShouldStartLoad;
				}
				else
				{
					Console.WriteLine("MPMainUI mpopenmolpaywindow empty dataString");
				}
			}
			else if (request.Url != null && request.Url.ToString().StartsWith(mpcloseallwindows))
			{
				if (mpBankUI != null)
				{
					mpBankUI.RemoveFromSuperview();
				}
				mpMOLPayUI.RemoveFromSuperview();
			}
			else if (request.Url != null && request.Url.ToString().StartsWith(mptransactionresults))
			{
				string base64String = request.Url.ToString().Replace(mptransactionresults, "");
				base64String = base64String.Replace("-", "+");
				base64String = base64String.Replace("_", "=");
				Console.WriteLine("MPMainUI mptransactionresults base64String = " + base64String);

				string dataString = Base64Decode(base64String);
				Console.WriteLine("MPMainUI mptransactionresults dataString = " + dataString);

				try
				{
					transactionResults = dataString;
					Dictionary<string, object> jsonResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(dataString);
					Console.WriteLine("MPMainUI jsonResult = " + JsonConvert.SerializeObject(jsonResult));

					Object requestType;
					jsonResult.TryGetValue("mp_request_type", out requestType);
					if (!jsonResult.ContainsKey("mp_request_type") || (string)requestType != "Receipt" || jsonResult.ContainsKey("error_code"))
					{
						Finish();
					}
					else
					{
						isClosingReceipt = true;
					}
				}
				catch (Exception)
				{
					Finish();
				}
			}
			else if (request.Url != null && request.Url.ToString().StartsWith(mprunscriptonpopup))
			{
				string base64String = request.Url.ToString().Replace(mprunscriptonpopup, "");
				base64String = base64String.Replace("-", "+");
				base64String = base64String.Replace("_", "=");
				Console.WriteLine("MPMainUI mprunscriptonpopup base64String = " + base64String);

				string jsString = Base64Decode(base64String);
				Console.WriteLine("MPMainUI mprunscriptonpopup jsString = " + jsString);

				if (mpBankUI != null)
				{
					mpBankUI.EvaluateJavascript("javascript:" + jsString);
					Console.WriteLine("mpBankUI EvaluateJavascript = " + "javascript:" + jsString);
				}
			}
			else if (request.Url != null && request.Url.ToString().StartsWith(mppinstructioncapture))
			{
				string base64String = request.Url.ToString().Replace(mppinstructioncapture, "");
				base64String = base64String.Replace("-", "+");
				base64String = base64String.Replace("_", "=");
				Console.WriteLine("MPMainUI mppinstructioncapture base64String = " + base64String);

				string jsString = Base64Decode(base64String);
				Console.WriteLine("MPMainUI mppinstructioncapture jsString = " + jsString);
				Dictionary<string, object> jsonResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsString);

				object base64ImageUrlData;
				jsonResult.TryGetValue("base64ImageUrlData", out base64ImageUrlData);
				object filename;
				jsonResult.TryGetValue("filename", out filename);

				byte[] imageData = System.Convert.FromBase64String(base64ImageUrlData.ToString());
				NSData data = NSData.FromArray(imageData);
				UIImage img = UIImage.LoadFromData(data);
				img.SaveToPhotosAlbum((image, error) =>
				{
					if (error == null || error.ToString() == "")
					{
						UIAlertView alert = new UIAlertView()
						{
							Title = "Info",
							Message = "Image saved"
						};
						alert.AddButton("OK");
						alert.Show();
					}
					else
					{ 
						UIAlertView alert = new UIAlertView()
						{
							Title = "Info",
							Message = "Image not saved"
						};
						alert.AddButton("OK");
						alert.Show();
					}	
				});
			}

			return true;
		}

		private void MPMainUILoadFinished(object sender, EventArgs e)
		{
			mpMainUI.EvaluateJavascript("updateSdkData(" + json + ")");
			mpMainUI.LoadFinished -= MPMainUILoadFinished;
		}

		private bool MPMOLPayUIShouldStartLoad(UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
		{
			if (request.Url != null && request.Url.ToString().Contains(molpayresulturl))
			{
				NativeWebRequestUrlUpdates(mpMainUI, request.Url.ToString());
			}
			else if (request.Url != null && request.Url.ToString().Contains(molpaynbepayurl))
			{
				hijackWindowOpen = true;
			}
			finishLoadUrl = request.Url.ToString();
			return true;
		}

		private void MPMOLPayUILoadFinished(object sender, EventArgs e)
		{
			if (hijackWindowOpen)
			{
				mpMOLPayUI.EvaluateJavascript("window.open = function (open) {" +
												"return function(url, name, features) {" +
													"window.location = url;" +
													"return window;" +
												"};" +
											"} (window.open); ");
			}
			NativeWebRequestUrlUpdates(mpMainUI, finishLoadUrl);

			Console.WriteLine("MPMOLPayUILoadFinished url = " + finishLoadUrl);

			if (finishLoadUrl != null && finishLoadUrl.Contains("intermediate_appTNG-EWALLET.php"))
            {
				string returnResult = mpMOLPayUI.EvaluateJavascript($"document.getElementById('systembrowserurl').innerHTML");
				string url = Base64Decode(returnResult);
				Console.WriteLine("MPMOLPayUILoadFinished returnResult = " + url);

				var canOpen = UIApplication.SharedApplication.CanOpenUrl(new NSUrl(url));

                if (canOpen)
				{
					UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
				}
			}
		}

		private void NativeWebRequestUrlUpdates(UIWebView webView, string url)
		{
			Dictionary<string, object> data = new Dictionary<string, object>();
			data.Add("requestPath", url);

			webView.EvaluateJavascript("nativeWebRequestUrlUpdates(" + JsonConvert.SerializeObject(data) + ")");
		}

		private string Base64Encode(string plainText)
		{
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return System.Convert.ToBase64String(plainTextBytes);
		}

		private string Base64Decode(string base64EncodedData)
		{
			var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
			return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
		}

		private void Finish()
		{
			mpMainUI.RemoveFromSuperview();
			callback(transactionResults);
		}
	}
}
