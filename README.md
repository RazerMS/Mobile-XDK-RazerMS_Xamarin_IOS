<!--
# license: Copyright © 2011-2016 MOLPay Sdn Bhd. All Rights Reserved. 
-->

# molpay-mobile-xdk-xamarin-ios

This is the complete and functional MOLPay Xamarin iOS payment module that is ready to be implemented into Xamarin iOS project through C# file drag and drop. An example application project (MOLPayXDKExample) is provided for MOLPayXDK Xamarin iOS integration reference.

This plugin provides an integrated MOLPay payment module that contains a wrapper 'MOLPay.cs' and an upgradable core as the 'molpay-mobile-xdk-www' folder, which the latter can be separately downloaded at https://github.com/MOLPay/molpay-mobile-xdk-www and update the local version.

## Recommended configurations

    - Xamarin Studio 6.0 ++

    - Json.NET

    - Minimum iOS target version: 7.0

## Installation

    Step 1 - Drag and drop MOLPay.cs into the project folder of your Xamarin iOS project

    Step 2 - Drag and drop molpay-mobile-xdk-www folder (can be separately downloaded at https://github.com/MOLPay/molpay-mobile-xdk-www) into Content folder of your Xamarin iOS project. Right click on each file in Xamarin Studio, go to Build Action and make sure BundleResource is checked

    Step 3 - Drag and drop custom.css into Content folder of your Xamarin iOS project. Right click on the file in Xamarin Studio, go to Build Action and make sure BundleResource is checked

    Step 4 - Add package Json.NET by going to Project -> Add NuGet Packages..., in the window that pops up, check Json.NET and click the Add Package button on the bottom right

    Step 5 - Add the result callback function
    public void MolpayCallback(string transactionResult)
    {
        Console.WriteLine("MolpayCallback transactionResult = " + transactionResult);
    }

## Using namespaces

    using System.Collections.Generic;
    using System.IO;
    using Foundation;
    using Newtonsoft.Json;
    using MOLPayXDK;

## Prepare the Payment detail object

    Dictionary<String, object> paymentDetails = new Dictionary<String, object>();
    // Mandatory String. A value more than '1.00'
    paymentDetails.Add(MOLPay.mp_amount, "");

    // Mandatory String. Values obtained from MOLPay
    paymentDetails.Add(MOLPay.mp_username, "");
    paymentDetails.Add(MOLPay.mp_password, "");
    paymentDetails.Add(MOLPay.mp_merchant_ID, "");
    paymentDetails.Add(MOLPay.mp_app_name, "");
    paymentDetails.Add(MOLPay.mp_verification_key, "");

    // Mandatory String. Payment values
    paymentDetails.Add(MOLPay.mp_order_ID, "");
    paymentDetails.Add(MOLPay.mp_currency, "");
    paymentDetails.Add(MOLPay.mp_country, "");

    // Optional String.
    paymentDetails.Add(MOLPay.mp_channel, ""); // Use 'multi' for all available channels option. For individual channel seletion, please refer to "Channel Parameter" in "Channel Lists" in the MOLPay API Spec for Merchant pdf. 
    paymentDetails.Add(MOLPay.mp_bill_description, "");
    paymentDetails.Add(MOLPay.mp_bill_name, "");
    paymentDetails.Add(MOLPay.mp_bill_email, "");
    paymentDetails.Add(MOLPay.mp_bill_mobile, "");
    paymentDetails.Add(MOLPay.mp_channel_editing, false); // Option to allow channel selection.
    paymentDetails.Add(MOLPay.mp_editing_enabled, false); // Option to allow billing information editing.

    // Optional for Escrow
    paymentDetails.Add(MOLPay.mp_is_escrow, ""); // Optional for Escrow, put "1" to enable escrow

    // Optional for credit card BIN restrictions
    String[] binlock = new String[] { "", "" };
    paymentDetails.Add(MOLPay.mp_bin_lock, binlock); // Optional for credit card BIN restrictions
    paymentDetails.Add(MOLPay.mp_bin_lock_err_msg, ""); // Optional for credit card BIN restrictions

    // For transaction request use only, do not use this on payment process
    paymentDetails.Add(MOLPay.mp_transaction_id, ""); // Optional, provide a valid cash channel transaction id here will display a payment instruction screen.
    paymentDetails.Add(MOLPay.mp_request_type, ""); // Optional, set 'Status' when performing a transactionRequest

    // Optional for customizing MOLPay UI
    paymentDetails.Add(MOLPay.mp_custom_css_url, Path.Combine(NSBundle.MainBundle.BundlePath, "Content/custom.css"));

    // Optional, set the token id to nominate a preferred token as the default selection, set "new" to allow new card only
    paymentDetails.Add(MOLPay.mp_preferred_token, "");

    // Optional, credit card transaction type, set "AUTH" to authorize the transaction
    paymentDetails.Add(MOLPay.mp_tcctype, "");

    // Optional, set true to process this transaction through the recurring api, please refer the MOLPay Recurring API pdf 
    paymentDetails.Add(MOLPay.mp_is_recurring, false);

## Start the payment module UI

    MOLPay molpay = new MOLPay(paymentDetails, MolpayCallback);
    molpay.Title = "MOLPayXDK";
    NavigationController.PushViewController(molpay, true);

## Support

Submit issue to this repository or email to our support@molpay.com

Merchant Technical Support / Customer Care : support@molpay.com<br>
Sales/Reseller Enquiry : sales@molpay.com<br>
Marketing Campaign : marketing@molpay.com<br>
Channel/Partner Enquiry : channel@molpay.com<br>
Media Contact : media@molpay.com<br>
R&D and Tech-related Suggestion : technical@molpay.com<br>
Abuse Reporting : abuse@molpay.com