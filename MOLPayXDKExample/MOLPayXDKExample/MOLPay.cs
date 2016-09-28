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
		public const String mp_amount = "mp_amount";
		public const String mp_username = "mp_username";
		public const String mp_password = "mp_password";
		public const String mp_merchant_ID = "mp_merchant_ID";
		public const String mp_app_name = "mp_app_name";
		public const String mp_order_ID = "mp_order_ID";
		public const String mp_currency = "mp_currency";
		public const String mp_country = "mp_country";
		public const String mp_verification_key = "mp_verification_key";
		public const String mp_channel = "mp_channel";
		public const String mp_bill_description = "mp_bill_description";
		public const String mp_bill_name = "mp_bill_name";
		public const String mp_bill_email = "mp_bill_email";
		public const String mp_bill_mobile = "mp_bill_mobile";
		public const String mp_channel_editing = "mp_channel_editing";
		public const String mp_editing_enabled = "mp_editing_enabled";
		public const String mp_transaction_id = "mp_transaction_id";
		public const String mp_request_type = "mp_request_type";
		public const String mp_is_escrow = "mp_is_escrow";
		public const String mp_bin_lock = "mp_bin_lock";
		public const String mp_bin_lock_err_msg = "mp_bin_lock_err_msg";
		public const String mp_custom_css_url = "mp_custom_css_url";
		public const String mp_preferred_token = "mp_preferred_token";
		public const String mp_tcctype = "mp_tcctype";
		public const String mp_is_recurring = "mp_is_recurring";
		public const String mp_sandbox_mode = "mp_sandbox_mode";
		public const String mp_allowed_channels = "mp_allowed_channels";

		private const String mpopenmolpaywindow = "mpopenmolpaywindow://";
		private const String mpcloseallwindows = "mpcloseallwindows://";
		private const String mptransactionresults = "mptransactionresults://";
		private const String mprunscriptonpopup = "mprunscriptonpopup://";
		private const String mppinstructioncapture = "mppinstructioncapture://";
		private const String molpayresulturl = "https://www.onlinepayment.com.my/MOLPay/result.php";
		private const String molpaynbepayurl = "https://www.onlinepayment.com.my/MOLPay/nbepay.php";
		private const String module_id = "module_id";
		private const String wrapper_version = "wrapper_version";
		private String finishLoadUrl;
		private Boolean isClosingReceipt = false;
		private Boolean hijackWindowOpen = false;
		private Action<string> callback;
		private String json;
		private String transactionResults;
		private UIWebView mpMainUI, mpMOLPayUI, mpBankUI;

		public MOLPay(Dictionary<String, object> paymentDetails, Action<string> callback)
		{
			paymentDetails.Add(module_id, "molpay-mobile-xdk-xamarin-ios");
			paymentDetails.Add(wrapper_version, "1");
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

			string fileName = "Content/molpay-mobile-xdk-www/index.html";
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
				String base64String = request.Url.ToString().Replace(mpopenmolpaywindow, "");
				base64String = base64String.Replace("-", "+");
				base64String = base64String.Replace("_", "=");
				Console.WriteLine("MPMainUI mpopenmolpaywindow base64String = " + base64String);

				String dataString = Base64Decode(base64String);
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
				String base64String = request.Url.ToString().Replace(mptransactionresults, "");
				base64String = base64String.Replace("-", "+");
				base64String = base64String.Replace("_", "=");
				Console.WriteLine("MPMainUI mptransactionresults base64String = " + base64String);

				String dataString = Base64Decode(base64String);
				Console.WriteLine("MPMainUI mptransactionresults dataString = " + dataString);

				try
				{
					transactionResults = dataString;
					Dictionary<String, object> jsonResult = JsonConvert.DeserializeObject<Dictionary<String, object>>(dataString);
					Console.WriteLine("MPMainUI jsonResult = " + JsonConvert.SerializeObject(jsonResult));

					Object requestType;
					jsonResult.TryGetValue("mp_request_type", out requestType);
					if (!jsonResult.ContainsKey("mp_request_type") || (String)requestType != "Receipt" || jsonResult.ContainsKey("error_code"))
					{
						Finish();
					}
					else
					{
						isClosingReceipt = true;
					}
				}
				catch (Exception ex)
				{
					Finish();
				}
			}
			else if (request.Url != null && request.Url.ToString().StartsWith(mprunscriptonpopup))
			{
				String base64String = request.Url.ToString().Replace(mprunscriptonpopup, "");
				base64String = base64String.Replace("-", "+");
				base64String = base64String.Replace("_", "=");
				Console.WriteLine("MPMainUI mprunscriptonpopup base64String = " + base64String);

				String jsString = Base64Decode(base64String);
				Console.WriteLine("MPMainUI mprunscriptonpopup jsString = " + jsString);

				if (mpBankUI != null)
				{
					mpBankUI.EvaluateJavascript("javascript:" + jsString);
					Console.WriteLine("mpBankUI EvaluateJavascript = " + "javascript:" + jsString);
				}
			}
			else if (request.Url != null && request.Url.ToString().StartsWith(mppinstructioncapture))
			{
				String base64String = request.Url.ToString().Replace(mppinstructioncapture, "");
				base64String = base64String.Replace("-", "+");
				base64String = base64String.Replace("_", "=");
				Console.WriteLine("MPMainUI mppinstructioncapture base64String = " + base64String);

				String jsString = Base64Decode(base64String);
				Console.WriteLine("MPMainUI mppinstructioncapture jsString = " + jsString);
				Dictionary<String, object> jsonResult = JsonConvert.DeserializeObject<Dictionary<String, object>>(jsString);

				object base64ImageUrlData;
				jsonResult.TryGetValue("base64ImageUrlData", out base64ImageUrlData);
				object filename;
				jsonResult.TryGetValue("filename", out filename);

				byte[] imageData = System.Convert.FromBase64String(base64ImageUrlData.ToString());
				NSData data = NSData.FromArray(imageData);
				UIImage img = UIImage.LoadFromData(data);
				img.SaveToPhotosAlbum((image, error) =>
				{
					var o = image as UIImage;
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
			if (request.Url != null && request.Url.ToString().StartsWith(molpayresulturl))
			{
				NativeWebRequestUrlUpdates(mpMainUI, request.Url.ToString());
			}
			else if (request.Url != null && request.Url.ToString().StartsWith(molpaynbepayurl))
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
			NativeWebRequestUrlUpdatesOnFinishLoad(mpMainUI, finishLoadUrl);
		}

		private void NativeWebRequestUrlUpdates(UIWebView webView, String url)
		{
			Dictionary<String, object> data = new Dictionary<String, object>();
			data.Add("requestPath", url);

			webView.EvaluateJavascript("nativeWebRequestUrlUpdates(" + JsonConvert.SerializeObject(data) + ")");
		}

		private void NativeWebRequestUrlUpdatesOnFinishLoad(UIWebView webView, String url)
		{
			Dictionary<String, object> data = new Dictionary<String, object>();
			data.Add("requestPath", url);

			webView.EvaluateJavascript("nativeWebRequestUrlUpdatesOnFinishLoad(" + JsonConvert.SerializeObject(data) + ")");
		}

		private String Base64Encode(String plainText)
		{
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return System.Convert.ToBase64String(plainTextBytes);
		}

		private String Base64Decode(String base64EncodedData)
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