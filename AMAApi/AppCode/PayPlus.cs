using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FarmsApi.Services
{
    public class PayPlus
    {
        public string _BaseUrl = "";
        public string _APIKey = "";
        public string _SecretKey = "";
        public string _PaymentPageUid = "";
        public string _TerminalUID = "";
        public string _CushierUID = "";

        public PayPlus()
        {
            _BaseUrl = ConfigurationSettings.AppSettings["BaseUrl"].ToString();
            _APIKey = ConfigurationSettings.AppSettings["APIKey"].ToString();
            _SecretKey = ConfigurationSettings.AppSettings["SecretKey"].ToString();
            _PaymentPageUid = ConfigurationSettings.AppSettings["PaymentPageUid"].ToString();
            _TerminalUID = ConfigurationSettings.AppSettings["TerminalUID"].ToString();
            _CushierUID = ConfigurationSettings.AppSettings["CushierUID"].ToString();
        }

        public async Task<CreateCustomerResponse> CustomerADD_PayPlus(Customer customer)
        {
            var json = JsonConvert.SerializeObject(customer);
            var res = await this.GenericACTION("Customers/Add", Method.POST, null, json);
            var result = JsonConvert.DeserializeObject<CreateCustomerResponse>(res);
            return result;
        }

        public async Task<PayPlusCreateTokenResponse> TokenADD_PayPlus(PayPlusCreateTokenRequest req)
        {
            req.TerminalUid = this._TerminalUID;

            var json = JsonConvert.SerializeObject(req);
            var res = await this.GenericACTION("Token/Add", Method.POST, null, json);
            var result = JsonConvert.DeserializeObject<PayPlusCreateTokenResponse>(res);
            return result;
        }

        public async Task<PayPlusRecurringResponse> RecurringPaymentsADD_PayPlus(PayPlusRecurringRequest req)
        {
            req.TerminalUid = this._TerminalUID;
            req.CashierUid = this._CushierUID;

            var json = JsonConvert.SerializeObject(req);
            var res = await this.GenericACTION("RecurringPayments/Add", Method.POST, null, json);
            var result = JsonConvert.DeserializeObject<PayPlusRecurringResponse>(res);
            return result;
        }

        public async Task<PayPlusRecurringResponse> RecurringPaymentsUpdate_PayPlus(PayPlusRecurringRequest req, string paymentuid)
        {
            req.TerminalUid = this._TerminalUID;
            req.CashierUid = this._CushierUID;

            var json = JsonConvert.SerializeObject(req);
            var res = await this.GenericACTION("RecurringPayments/Update/" + paymentuid, Method.POST, null, json);
            var result = JsonConvert.DeserializeObject<PayPlusRecurringResponse>(res);
            return result;
        }

        public async Task<PayPlusRecurringResponse> RecurringPaymentsDelete_PayPlus(string paymentuid)
        {
            var req = new PayPlusRecurringRequest
            {
                TerminalUid = this._TerminalUID
            };

            var json = JsonConvert.SerializeObject(req);
            var res = await this.GenericACTION("RecurringPayments/DeleteRecurring/" + paymentuid, Method.POST, null, json);
            var result = JsonConvert.DeserializeObject<PayPlusRecurringResponse>(res);
            return result;
        }

        public async Task<PayPlusAddProductResponse> ProductsADD_PayPlus(PayPlusAddProductRequest req)
        {
            var json = JsonConvert.SerializeObject(req);
            var res = await this.GenericACTION("Products/Add", Method.POST, null, json);
            var result = JsonConvert.DeserializeObject<PayPlusAddProductResponse>(res);
            return result;
        }

        public Task<string> GenericACTION(string requestUrl, Method method, string token = null, object body = null, bool retryOn401 = true)
        {

            ServicePointManager.SecurityProtocol =
        SecurityProtocolType.Tls12 |
        SecurityProtocolType.Tls11 |
        SecurityProtocolType.Tls;


            var client = new RestClient(_BaseUrl);
            client.Timeout = -1;

            var req = new RestRequest(requestUrl, method);


            // 👇 בדיוק כמו ב-.NET 8
            if (body != null)
            {
                req.AddJsonBody(body);
            }

            //if (body != null)
            //{
            //    // body הוא כבר JSON string
            //    req.AddParameter("application/json", body, ParameterType.RequestBody);
            //}

            if (!string.IsNullOrEmpty(token))
                req.AddHeader("Authorization", "Bearer " + token);

            req.AddHeader("accept", "application/json");
            req.AddHeader("api-key", _APIKey);
            req.AddHeader("secret-key", _SecretKey);
                        // שים לב – בלי await, קריאה סינכרונית
            IRestResponse resp = client.Execute(req);

            // מחזיר Task בשביל השאר שנשאר async
            return Task.FromResult(resp.Content);
        }


        public async Task<PayPlusInvoicesResponse> GetDocuments_PayPlus(PayPlusTransactionFilterRequest payPlusTransactionFilterRequest)
        {
            var json = JsonConvert.SerializeObject(payPlusTransactionFilterRequest);
            var res = await this.GenericACTION("Invoice/GetDocuments", Method.POST, null, json);
            var result = JsonConvert.DeserializeObject<PayPlusInvoicesResponse>(res);
            return result;
        }
    }

    //********************************************** בקשה לדף תשלום ***************************************
    public class GeneratePaymentLinkRequest
    {
        [JsonPropertyName("payment_page_uid")]
        public string PaymentPageUid { get; set; }

        [JsonPropertyName("charge_method")]
        public ChargeMethod? ChargeMethodCode { get; set; }

        [JsonPropertyName("charge_default")]
        public string ChargeDefault { get; set; }

        [JsonPropertyName("hide_other_charge_methods")]
        public bool? HideOtherChargeMethods { get; set; }

        [JsonPropertyName("language_code")]
        public string LanguageCode { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("non_voucher_minimum_amount")]
        public int? NonVoucherMinimumAmount { get; set; }

        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonPropertyName("sendEmailApproval")]
        public bool? SendEmailApproval { get; set; }

        [JsonPropertyName("sendEmailFailure")]
        public bool? SendEmailFailure { get; set; }

        [JsonPropertyName("expiry_datetime")]
        public string ExpiryDateTime { get; set; }

        [JsonPropertyName("refURL_success")]
        public string RefUrlSuccess { get; set; }

        [JsonPropertyName("refURL_failure")]
        public string RefUrlFailure { get; set; }

        [JsonPropertyName("refURL_cancel")]
        public string RefUrlCancel { get; set; }

        [JsonPropertyName("refURL_callback")]
        public string RefUrlCallback { get; set; }

        [JsonPropertyName("send_failure_callback")]
        public bool? SendFailureCallback { get; set; }

        [JsonPropertyName("custom_invoice_name")]
        public string CustomInvoiceName { get; set; }

        [JsonPropertyName("create_token")]
        public bool? CreateToken { get; set; }

        [JsonPropertyName("initial_invoice")]
        public bool? InitialInvoice { get; set; }

        [JsonPropertyName("invoice_language")]
        public bool? InvoiceLanguage { get; set; }

        [JsonPropertyName("paying_vat")]
        public bool? PayingVat { get; set; }

        [JsonPropertyName("hide_payments_field")]
        public bool? HidePaymentsField { get; set; }

        [JsonPropertyName("payments")]
        public int? Payments { get; set; }

        [JsonPropertyName("payments_credit")]
        public bool? PaymentsCredit { get; set; }

        [JsonPropertyName("payments_selected")]
        public int? PaymentsSelected { get; set; }

        [JsonPropertyName("payments_first_amount")]
        public int? PaymentsFirstAmount { get; set; }

        [JsonPropertyName("hide_identification_id")]
        public bool? HideIdentificationId { get; set; }

        [JsonPropertyName("send_customer_success_sms")]
        public bool? SendCustomerSuccessSms { get; set; }

        [JsonPropertyName("customer_failure_sms")]
        public bool? CustomerFailureSms { get; set; }

        [JsonPropertyName("add_user_information")]
        public bool? AddUserInformation { get; set; }

        [JsonPropertyName("allowed_cards")]
        public List<string> AllowedCards { get; set; }

        [JsonPropertyName("allowed_bins")]
        public List<int> AllowedBins { get; set; }

        [JsonPropertyName("allowed_charge_methods")]
        public List<string> AllowedChargeMethods { get; set; }

        [JsonPropertyName("more_info")]
        public string MoreInfo { get; set; }

        [JsonPropertyName("more_info_2")]
        public string MoreInfo2 { get; set; }

        [JsonPropertyName("more_info_3")]
        public string MoreInfo3 { get; set; }

        [JsonPropertyName("more_info_4")]
        public string MoreInfo4 { get; set; }

        [JsonPropertyName("more_info_5")]
        public string MoreInfo5 { get; set; }

        [JsonPropertyName("create_hash")]
        public string CreateHash { get; set; }

        [JsonPropertyName("show_more_info")]
        public string ShowMoreInfo { get; set; }

        [JsonPropertyName("support_track2")]
        public bool? SupportTrack2 { get; set; }

        [JsonPropertyName("close_doc")]
        public string CloseDoc { get; set; }

        [JsonPropertyName("customer")]
        public CustomerDto Customer { get; set; }

        [JsonPropertyName("items")]
        public List<ItemDtoPayPlus> Items { get; set; }

        [JsonPropertyName("recurring_settings")]
        public RecurringSettingsDto RecurringSettings { get; set; }

        [JsonPropertyName("secure3d")]
        public Secure3dDto Secure3d { get; set; }

        [JsonPropertyName("allowed_issuers")]
        public List<string> AllowedIssuers { get; set; }

        [JsonPropertyName("invoice_integration_uid")]
        public string InvoiceIntegrationUid { get; set; }

        [JsonPropertyName("cashier_uid")]
        public string CashierUid { get; set; }

        public void ApplyDefaults()
        {
            if (CurrencyCode == null) CurrencyCode = "ILS";

            if (!SendEmailApproval.HasValue) SendEmailApproval = true;
            if (!SendEmailFailure.HasValue) SendEmailFailure = false;

            if (!ChargeMethodCode.HasValue) ChargeMethodCode = ChargeMethod.Charge;
            if (!HideOtherChargeMethods.HasValue) HideOtherChargeMethods = false;
            if (LanguageCode == null) LanguageCode = "he";
            if (!NonVoucherMinimumAmount.HasValue) NonVoucherMinimumAmount = 0;
            if (ExpiryDateTime == null) ExpiryDateTime = "30";
            if (!SendFailureCallback.HasValue) SendFailureCallback = false;

            if (!CreateToken.HasValue) CreateToken = false;
            if (!InitialInvoice.HasValue) InitialInvoice = true;
            if (!InvoiceLanguage.HasValue) InvoiceLanguage = false;
            if (!PayingVat.HasValue) PayingVat = true;

            if (!HidePaymentsField.HasValue) HidePaymentsField = false;
            if (!Payments.HasValue) Payments = 5;
            if (!PaymentsCredit.HasValue) PaymentsCredit = false;
            if (!PaymentsSelected.HasValue) PaymentsSelected = 1;
            if (!PaymentsFirstAmount.HasValue) PaymentsFirstAmount = 5;

            if (!HideIdentificationId.HasValue) HideIdentificationId = false;
            if (!SendCustomerSuccessSms.HasValue) SendCustomerSuccessSms = false;
            if (!CustomerFailureSms.HasValue) CustomerFailureSms = false;
            if (!AddUserInformation.HasValue) AddUserInformation = false;

            if (AllowedCards == null)
                AllowedCards = new List<string> { "mastercard", "visa" };

            if (AllowedChargeMethods == null)
                AllowedChargeMethods = new List<string> { "credit-card", "google-pay" };

            if (!SupportTrack2.HasValue) SupportTrack2 = false;
        }
    }

    public enum ChargeMethod
    {
        Check = 0,
        Charge = 1,
        Approval = 2,
        RecurringPayments = 3,
        Refund = 4,
        Token = 5
    }

    public class CustomerDto
    {
        [JsonPropertyName("customer_uid")] public string CustomerUid { get; set; }
        [JsonPropertyName("customer_name")] public string CustomerName { get; set; }
        [JsonPropertyName("email")] public string Email { get; set; }
        [JsonPropertyName("phone")] public string Phone { get; set; }
    }

    public class ItemDtoPayPlus
    {
        [JsonPropertyName("product_uid")] public string ProductUid { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("price")] public decimal? Price { get; set; }
        [JsonPropertyName("quantity")] public int? Quantity { get; set; }
        [JsonPropertyName("vat_type")] public string VatType { get; set; }
        [JsonPropertyName("barcode")] public string Barcode { get; set; }
    }

    public class RecurringSettingsDto
    {
        [JsonPropertyName("recurring_type")] public int? RecurringType { get; set; }
        [JsonPropertyName("recurring_range")] public int? RecurringRange { get; set; }
        [JsonPropertyName("start_date")] public string StartDate { get; set; }
        [JsonPropertyName("amount")] public decimal? Amount { get; set; }
    }

    public class Secure3dDto
    {
        [JsonPropertyName("enabled")] public bool? Enabled { get; set; }
        [JsonPropertyName("version")] public string Version { get; set; }
    }

    //************************************************** תשובה מבקשה לדף תשלום ***********************************
    public class PayPlusPaymentPageResponse
    {
        [JsonPropertyName("results")]
        public PayPlusResult Results { get; set; }

        [JsonPropertyName("data")]
        public PayPlusPaymentData Data { get; set; }
    }

    public class PayPlusResult
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class PayPlusPaymentData
    {
        [JsonPropertyName("page_request_uid")]
        public string PageRequestUid { get; set; }

        [JsonPropertyName("payment_page_link")]
        public string PaymentPageLink { get; set; }

        [JsonPropertyName("qr_code_image")]
        public string QrCodeImage { get; set; }

        [JsonPropertyName("hosted_fields_uuid")]
        public string HostedFieldsUuid { get; set; }
    }

    //************************************************ CallBack מסליקה *************************************
    public class CallbackData
    {
        [JsonPropertyName("transaction_type")]
        public string TransactionType { get; set; }

        [JsonPropertyName("transaction")]
        public Transaction Transaction { get; set; }

        [JsonPropertyName("data")]
        public CallbackInnerData Data { get; set; }

        [JsonPropertyName("invoice")]
        public Invoice Invoice { get; set; }
    }

    public class Transaction
    {
        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        [JsonPropertyName("uid_emv")]
        public string UidEmv { get; set; }

        [JsonPropertyName("payment_page_request_uid")]
        public string PaymentPageRequestUid { get; set; }

        [JsonPropertyName("number")]
        public string Number { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("status_code")]
        public string StatusCode { get; set; }

        [JsonPropertyName("amount")]
        public decimal? Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("credit_terms")]
        public string CreditTerms { get; set; }

        [JsonPropertyName("paramj")]
        public int? Paramj { get; set; }

        [JsonPropertyName("rrn")]
        public string Rrn { get; set; }

        [JsonPropertyName("payments")]
        public PaymentsCallback Payments { get; set; }

        [JsonPropertyName("secure3D")]
        public Secure3D Secure3D { get; set; }

        [JsonPropertyName("approval_number")]
        public string ApprovalNumber { get; set; }

        [JsonPropertyName("voucher_number")]
        public string VoucherNumber { get; set; }

        [JsonPropertyName("more_info")]
        public string MoreInfo { get; set; }

        [JsonPropertyName("more_info_1")]
        public string MoreInfo1 { get; set; }

        [JsonPropertyName("more_info_2")]
        public string MoreInfo2 { get; set; }

        [JsonPropertyName("more_info_3")]
        public string MoreInfo3 { get; set; }

        [JsonPropertyName("more_info_4")]
        public string MoreInfo4 { get; set; }

        [JsonPropertyName("more_info_5")]
        public string MoreInfo5 { get; set; }

        [JsonPropertyName("add_data")]
        public string AddData { get; set; }

        [JsonPropertyName("original_amount_currency_dcc")]
        public string OriginalAmountCurrencyDcc { get; set; }
    }

    public class PaymentsCallback
    {
        [JsonPropertyName("number_of_payments")]
        public int? NumberOfPayments { get; set; }

        [JsonPropertyName("first_payment_amount")]
        public int? FirstPaymentAmount { get; set; }

        [JsonPropertyName("rest_payments_amount")]
        public int? RestPaymentsAmount { get; set; }
    }

    public class Secure3D
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("tracking")]
        public string Tracking { get; set; }
    }

    public class CallbackInnerData
    {
        [JsonPropertyName("customer_uid")]
        public string CustomerUid { get; set; }

        [JsonPropertyName("customer_email")]
        public string CustomerEmail { get; set; }

        [JsonPropertyName("terminal_uid")]
        public string TerminalUid { get; set; }

        [JsonPropertyName("cashier_uid")]
        public string CashierUid { get; set; }

        [JsonPropertyName("cashier_name")]
        public string CashierName { get; set; }

        [JsonPropertyName("items")]
        public List<CallbackItem> Items { get; set; }

        [JsonPropertyName("card_information")]
        public CardInformation CardInformation { get; set; }

        [JsonPropertyName("invoice")]
        public Invoice Invoice { get; set; }
    }

    public class CallbackItem
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("amount_pay")]
        public decimal? AmountPay { get; set; }

        [JsonPropertyName("discount_amount")]
        public int? DiscountAmount { get; set; }

        [JsonPropertyName("discount_type")]
        public string DiscountType { get; set; }

        [JsonPropertyName("discount_value")]
        public string DiscountValue { get; set; }

        [JsonPropertyName("quantity")]
        public int? Quantity { get; set; }

        [JsonPropertyName("quantity_price")]
        public decimal? QuantityPrice { get; set; }

        [JsonPropertyName("product_uid")]
        public string ProductUid { get; set; }

        [JsonPropertyName("product_variant_uid")]
        public string ProductVariantUid { get; set; }

        [JsonPropertyName("vat")]
        public decimal? Vat { get; set; }

        [JsonPropertyName("barcode")]
        public string Barcode { get; set; }
    }

    public class CardInformation
    {
        [JsonPropertyName("card_bin")]
        public string CardBin { get; set; }

        [JsonPropertyName("card_holder_name")]
        public string CardHolderName { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("four_digits")]
        public string FourDigits { get; set; }

        [JsonPropertyName("expiry_month")]
        public string ExpiryMonth { get; set; }

        [JsonPropertyName("expiry_year")]
        public string ExpiryYear { get; set; }

        [JsonPropertyName("clearing_id")]
        public int? ClearingId { get; set; }

        [JsonPropertyName("brand_id")]
        public int? BrandId { get; set; }

        [JsonPropertyName("issuer_id")]
        public int? IssuerId { get; set; }

        [JsonPropertyName("card_foreign")]
        public string CardForeign { get; set; }

        [JsonPropertyName("identification_number")]
        public string IdentificationNumber { get; set; }
    }

    public class Invoice
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("docu_number")]
        public string DocuNumber { get; set; }

        [JsonPropertyName("original_url")]
        public string OriginalUrl { get; set; }

        [JsonPropertyName("copy_url")]
        public string CopyUrl { get; set; }

        [JsonPropertyName("integrator_name")]
        public string IntegratorName { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("brand_name")]
        public string BrandName { get; set; }
    }

    //******************************************************* לקוח ******************************
    public class Customer
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("customer_name")]
        public string CustomerName { get; set; }

        [JsonPropertyName("paying_vat")]
        public bool? PayingVat { get; set; }

        [JsonPropertyName("vat_number")]
        public int? VatNumber { get; set; }

        [JsonPropertyName("customer_number")]
        public string CustomerNumber { get; set; }

        [JsonPropertyName("notes")]
        public string Notes { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("contacts")]
        public List<CustomerContact> Contacts { get; set; }

        [JsonPropertyName("business_address")]
        public string BusinessAddress { get; set; }

        [JsonPropertyName("business_city")]
        public string BusinessCity { get; set; }

        [JsonPropertyName("business_postal_code")]
        public string BusinessPostalCode { get; set; }

        [JsonPropertyName("business_country_iso")]
        public string BusinessCountryIso { get; set; }

        [JsonPropertyName("subject_code")]
        public string SubjectCode { get; set; }

        [JsonPropertyName("communication_email")]
        public string CommunicationEmail { get; set; }
    }

    public class CustomerContact
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }
    }

    //******************************************************** תשובה מיצירת לקוח *****************************
    public class CreateCustomerResponse
    {
        [JsonPropertyName("results")]
        public ResultInfo Results { get; set; }

        [JsonPropertyName("data")]
        public CreateCustomerData Data { get; set; }
    }

    public class ResultInfo
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("code")]
        public int? Code { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class CreateCustomerData
    {
        [JsonPropertyName("customer_uid")]
        public string CustomerUid { get; set; }
    }

    //******************************************************** טוקן *****************************
    public class PayPlusCreateTokenRequest
    {
        [JsonPropertyName("terminal_uid")]
        public string TerminalUid { get; set; }

        [JsonPropertyName("customer_uid")]
        public string CustomerUid { get; set; }

        [JsonPropertyName("card_date_mmyy")]
        public string CardDateMMyy { get; set; }

        [JsonPropertyName("credit_card_number")]
        public string CreditCardNumber { get; set; }

        [JsonPropertyName("identification_number")]
        public string IdentificationNumber { get; set; }

        [JsonPropertyName("previous_uid")]
        public string PreviousUid { get; set; }
    }

    //************************************************* תשובה בעת יצירת טוקן *********************************
    public class PayPlusCreateTokenResponse
    {
        [JsonPropertyName("results")]
        public PayPlusResult Results { get; set; }

        [JsonPropertyName("data")]
        public PayPlusTokenData Data { get; set; }
    }

    public class PayPlusTokenData
    {
        [JsonPropertyName("card_uid")]
        public string CardUid { get; set; }
    }

    //***********************************************************  הגדרות הוראת קבע  *************************************************
    public class PayPlusRecurringRequest
    {
        [JsonPropertyName("terminal_uid")]
        public string TerminalUid { get; set; }

        [JsonPropertyName("customer_uid")]
        public string CustomerUid { get; set; }

        [JsonPropertyName("card_token")]
        public string CardToken { get; set; }

        [JsonPropertyName("bank_account_uid")]
        public string BankAccountUid { get; set; }

        [JsonPropertyName("company_bank_account_uid")]
        public string CompanyBankAccountUid { get; set; }

        [JsonPropertyName("cashier_uid")]
        public string CashierUid { get; set; }

        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonPropertyName("instant_first_payment")]
        public bool InstantFirstPayment { get; set; }

        [JsonPropertyName("valid")]
        public bool Valid { get; set; }

        [JsonPropertyName("recurring_type")]
        public int RecurringType { get; set; }

        [JsonPropertyName("recurring_range")]
        public int RecurringRange { get; set; }

        [JsonPropertyName("number_of_charges")]
        public int NumberOfCharges { get; set; }

        [JsonPropertyName("start_date")]
        public string StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public string EndDate { get; set; }

        [JsonPropertyName("items")]
        public List<RecurringItem> Items { get; set; }

        [JsonPropertyName("one_time_items")]
        public List<RecurringItem> OneTimeItems { get; set; }

        [JsonPropertyName("one_time_charge_date")]
        public string OneTimeChargeDate { get; set; }

        [JsonPropertyName("successful_invoice")]
        public bool SuccessfulInvoice { get; set; }

        [JsonPropertyName("send_customer_success_email")]
        public bool SendCustomerSuccessEmail { get; set; }

        [JsonPropertyName("customer_failure_email")]
        public bool CustomerFailureEmail { get; set; }

        [JsonPropertyName("send_customer_success_sms")]
        public bool SendCustomerSuccessSms { get; set; }

        [JsonPropertyName("customer_failure_sms")]
        public bool CustomerFailureSms { get; set; }

        [JsonPropertyName("extra_info")]
        public string ExtraInfo { get; set; }

        public PayPlusRecurringRequest()
        {
            CurrencyCode = "ILS";
            InstantFirstPayment = true;
            Valid = true;
            RecurringType = 2;
            RecurringRange = 1;
            NumberOfCharges = 0;
            SuccessfulInvoice = true;
            Items = new List<RecurringItem>();
        }
    }

    public class RecurringItem
    {
        [JsonPropertyName("product_uid")]
        public string ProductUid { get; set; }

        [JsonPropertyName("quantity")]
        public decimal Quantity { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("discount_type")]
        public string DiscountType { get; set; }

        [JsonPropertyName("discount_value")]
        public decimal? DiscountValue { get; set; }

        [JsonPropertyName("product_invoice_extra_details")]
        public string ProductInvoiceExtraDetails { get; set; }

        public RecurringItem()
        {
            Quantity = 1;
            Price = 0;
        }
    }

    //***********************************************************  תגובה עבור יצירת הוראת קבע  *************************************************
    public class PayPlusRecurringResponse
    {
        [JsonPropertyName("results")]
        public PayPlusRecurringResult Results { get; set; }

        [JsonPropertyName("data")]
        public PayPlusRecurringData Data { get; set; }
    }

    public class PayPlusRecurringResult
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("code")]
        public int? Code { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class PayPlusRecurringData
    {
        [JsonPropertyName("recurring_payment_uid")]
        public string RecurringPaymentUid { get; set; }
    }

    //***********************************************************    יצירת Product  *************************************************
    public class PayPlusAddProductRequest
    {
        [JsonPropertyName("category_uids")]
        public List<string> CategoryUids { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("valid")]
        public bool? Valid { get; set; }

        [JsonPropertyName("price")]
        public decimal? Price { get; set; }

        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonPropertyName("vat_type")]
        public int? VatType { get; set; }

        [JsonPropertyName("barcode")]
        public string Barcode { get; set; }

        [JsonPropertyName("value")]
        public decimal? Value { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("fixed_amount_discount")]
        public decimal? FixedAmountDiscount { get; set; }

        [JsonPropertyName("percentage_discount")]
        public decimal? PercentageDiscount { get; set; }

        [JsonPropertyName("guide_document_url")]
        public string GuideDocumentUrl { get; set; }

        public PayPlusAddProductRequest()
        {
            Valid = true;
            CurrencyCode = "ILS";
            VatType = 0;
        }
    }

    //***********************************************************  תגובה עבור יצירת Product  *************************************************
    public class PayPlusAddProductResponse
    {
        [JsonPropertyName("results")]
        public PayPlusAddProductResult Results { get; set; }

        [JsonPropertyName("data")]
        public PayPlusAddProductData Data { get; set; }
    }

    public class PayPlusAddProductResult
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("code")]
        public int? Code { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class PayPlusAddProductData
    {
        [JsonPropertyName("product_uid")]
        public string ProductUid { get; set; }

        [JsonPropertyName("product_variant_uid")]
        public string ProductVariantUid { get; set; }
    }

    //************************************************************* טרנזקציות ****************************************************************
    public class PayPlusChargeRequest
    {
        [JsonPropertyName("terminal_uid")]
        public string TerminalUid { get; set; }

        [JsonPropertyName("cashier_uid")]
        public string CashierUid { get; set; }

        [JsonPropertyName("amount")]
        public decimal? Amount { get; set; }

        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonPropertyName("credit_terms")]
        public int CreditTerms { get; set; }

        [JsonPropertyName("use_token")]
        public bool UseToken { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("initial_invoice")]
        public bool InitialInvoice { get; set; }

        [JsonPropertyName("customer_name_invoice")]
        public string CustomerNameInvoice { get; set; }

        [JsonPropertyName("create_token")]
        public bool CreateToken { get; set; }

        [JsonPropertyName("add_data")]
        public string AddData { get; set; }

        [JsonPropertyName("deferMonths")]
        public int DeferMonths { get; set; }

        [JsonPropertyName("original_terminal_uid")]
        public string OriginalTerminalUid { get; set; }

        [JsonPropertyName("customer_uid")]
        public string CustomerUid { get; set; }

        [JsonPropertyName("id")]
        public string IdentificationNumber { get; set; }

        [JsonPropertyName("customer")]
        public PayPlusChargeCustomer Customer { get; set; }

        [JsonPropertyName("products")]
        public List<PayPlusChargeProduct> Products { get; set; }

        [JsonPropertyName("credit_card")]
        public PayPlusChargeCard CreditCard { get; set; }

        [JsonPropertyName("payments")]
        public PayPlusChargePayments Payments { get; set; }

        [JsonPropertyName("extra_info")]
        public string ExtraInfo { get; set; }

        [JsonPropertyName("more_info_1")]
        public string MoreInfo1 { get; set; }

        [JsonPropertyName("more_info_2")]
        public string MoreInfo2 { get; set; }

        [JsonPropertyName("more_info_3")]
        public string MoreInfo3 { get; set; }

        [JsonPropertyName("more_info_4")]
        public string MoreInfo4 { get; set; }

        [JsonPropertyName("more_info_5")]
        public string MoreInfo5 { get; set; }

        [JsonPropertyName("self_secure_3ds")]
        public PayPlusSelfSecure3ds SelfSecure3ds { get; set; }

        public PayPlusChargeRequest()
        {
            CurrencyCode = "ILS";
            CreditTerms = 1;
            UseToken = false;
            InitialInvoice = true;
            CreateToken = false;
            DeferMonths = 0;
        }
    }

    public class PayPlusChargeCustomer
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("customer_name")]
        public string CustomerName { get; set; }
    }

    public class PayPlusChargeProduct
    {
        [JsonPropertyName("product_uid")]
        public string ProductUid { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("quantity")]
        public decimal? Quantity { get; set; }

        [JsonPropertyName("price")]
        public decimal? Price { get; set; }

        [JsonPropertyName("discount_type")]
        public string DiscountType { get; set; }

        [JsonPropertyName("discount_value")]
        public decimal? DiscountValue { get; set; }

        [JsonPropertyName("product_invoice_extra_details")]
        public string ProductInvoiceExtraDetails { get; set; }
    }

    public class PayPlusChargeCard
    {
        [JsonPropertyName("auth_number")]
        public string AuthNumber { get; set; }

        [JsonPropertyName("number")]
        public string Number { get; set; }

        [JsonPropertyName("exp_mm")]
        public string ExpMm { get; set; }

        [JsonPropertyName("exp_yy")]
        public string ExpYy { get; set; }

        [JsonPropertyName("cvv")]
        public string Cvv { get; set; }
    }

    public class PayPlusChargePayments
    {
        [JsonPropertyName("number")]
        public int? Number { get; set; }

        [JsonPropertyName("first_amount")]
        public decimal? FirstAmount { get; set; }

        [JsonPropertyName("nonfirst_amount")]
        public decimal? NonFirstAmount { get; set; }
    }

    public class PayPlusSelfSecure3ds
    {
        [JsonPropertyName("cavv_ucaf")]
        public string CavvUcaf { get; set; }

        [JsonPropertyName("eci")]
        public string Eci { get; set; }

        [JsonPropertyName("external_trans_uid")]
        public string ExternalTransUid { get; set; }

        [JsonPropertyName("external_uid")]
        public string ExternalUid { get; set; }
    }

    //************************************************************************  טרנזקציות תגובות **********************************************************
    public class PayPlusChargeResponse
    {
        [JsonPropertyName("results")]
        public PayPlusChargeResult Results { get; set; }

        [JsonPropertyName("data")]
        public PayPlusChargeData Data { get; set; }
    }

    public class PayPlusChargeResult
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("code")]
        public int? Code { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class PayPlusChargeData
    {
        [JsonPropertyName("transaction")]
        public PayPlusTransaction Transaction { get; set; }

        [JsonPropertyName("data")]
        public PayPlusChargeInnerData InnerData { get; set; }
    }

    public class PayPlusTransaction
    {
        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        [JsonPropertyName("uid_emv")]
        public string UidEmv { get; set; }

        [JsonPropertyName("number")]
        public string Number { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("status_code")]
        public string StatusCode { get; set; }

        [JsonPropertyName("amount")]
        public decimal? Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("credit_terms")]
        public string CreditTerms { get; set; }

        [JsonPropertyName("paramj")]
        public int? Paramj { get; set; }

        [JsonPropertyName("rrn")]
        public string Rrn { get; set; }

        [JsonPropertyName("payments")]
        public PaymentsCallback Payments { get; set; }

        [JsonPropertyName("secure3D")]
        public Secure3D Secure3D { get; set; }

        [JsonPropertyName("approval_number")]
        public string ApprovalNumber { get; set; }

        [JsonPropertyName("voucher_number")]
        public string VoucherNumber { get; set; }

        [JsonPropertyName("more_info")]
        public string MoreInfo { get; set; }

        [JsonPropertyName("more_info_1")]
        public string MoreInfo1 { get; set; }

        [JsonPropertyName("more_info_2")]
        public string MoreInfo2 { get; set; }

        [JsonPropertyName("more_info_3")]
        public string MoreInfo3 { get; set; }

        [JsonPropertyName("more_info_4")]
        public string MoreInfo4 { get; set; }

        [JsonPropertyName("more_info_5")]
        public string MoreInfo5 { get; set; }

        [JsonPropertyName("add_data")]
        public string AddData { get; set; }

        [JsonPropertyName("original_amount_currency_dcc")]
        public string OriginalAmountCurrencyDcc { get; set; }
    }

    public class PayPlusChargeInnerData
    {
        [JsonPropertyName("customer_uid")]
        public string CustomerUid { get; set; }

        [JsonPropertyName("customer_email")]
        public string CustomerEmail { get; set; }

        [JsonPropertyName("terminal_uid")]
        public string TerminalUid { get; set; }

        [JsonPropertyName("cashier_uid")]
        public string CashierUid { get; set; }

        [JsonPropertyName("cashier_name")]
        public string CashierName { get; set; }

        [JsonPropertyName("items")]
        public List<PayPlusItem> Items { get; set; }

        [JsonPropertyName("card_information")]
        public PayPlusCardInformation CardInformation { get; set; }
    }

    public class PayPlusItem
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("amount_pay")]
        public decimal? AmountPay { get; set; }

        [JsonPropertyName("discount_amount")]
        public decimal? DiscountAmount { get; set; }

        [JsonPropertyName("discount_type")]
        public string DiscountType { get; set; }

        [JsonPropertyName("discount_value")]
        public decimal? DiscountValue { get; set; }

        [JsonPropertyName("quantity")]
        public int? Quantity { get; set; }

        [JsonPropertyName("quantity_price")]
        public decimal? QuantityPrice { get; set; }

        [JsonPropertyName("product_uid")]
        public string ProductUid { get; set; }

        [JsonPropertyName("product_variant_uid")]
        public string ProductVariantUid { get; set; }

        [JsonPropertyName("vat")]
        public decimal? Vat { get; set; }

        [JsonPropertyName("barcode")]
        public string Barcode { get; set; }
    }

    public class PayPlusCardInformation
    {
        [JsonPropertyName("card_bin")]
        public string CardBin { get; set; }

        [JsonPropertyName("card_holder_name")]
        public string CardHolderName { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("four_digits")]
        public string FourDigits { get; set; }

        [JsonPropertyName("expiry_month")]
        public string ExpiryMonth { get; set; }

        [JsonPropertyName("expiry_year")]
        public string ExpiryYear { get; set; }

        [JsonPropertyName("clearing_id")]
        public int? ClearingId { get; set; }

        [JsonPropertyName("brand_id")]
        public int? BrandId { get; set; }

        [JsonPropertyName("issuer_id")]
        public int? IssuerId { get; set; }

        [JsonPropertyName("card_foreign")]
        public string CardForeign { get; set; }

        [JsonPropertyName("identification_number")]
        public string IdentificationNumber { get; set; }
    }

    //**************************************************  סינון טרנזקציות / מסמכים  ***********************************
    public class PayPlusTransactionFilterRequest
    {
        [JsonPropertyName("transaction_uid")]
        public string TransactionUid { get; set; }

        [JsonPropertyName("filter")]
        public PayPlusFilter Filter { get; set; }

        public PayPlusTransactionFilterRequest()
        {
            Filter = new PayPlusFilter();
        }
    }

    public class PayPlusFilter
    {
        [JsonPropertyName("fromDate")]
        public string FromDate { get; set; }

        [JsonPropertyName("untilDate")]
        public string UntilDate { get; set; }
    }

    public class PayPlusInvoicesResponse
    {
        [JsonPropertyName("invoices")]
        public List<PayPlusInvoice> Invoices { get; set; }
    }

    public class PayPlusInvoice
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("original_doc_url")]
        public string OriginalDocUrl { get; set; }

        [JsonPropertyName("copy_doc_url")]
        public string CopyDocUrl { get; set; }
    }

    //****************************************************************************************** Webhook
    public class WebhookPayload
    {
        [JsonPropertyName("transaction_type")]
        public string TransactionType { get; set; }

        [JsonPropertyName("transaction")]
        public TransactionWebhook Transaction { get; set; }

        [JsonPropertyName("data")]
        public DataBlock Data { get; set; }

        [JsonPropertyName("invoice")]
        public InvoiceWebhook Invoice { get; set; }
    }

    public class TransactionWebhook
    {
        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        [JsonPropertyName("uid_emv")]
        public string UidEmv { get; set; }

        [JsonPropertyName("number")]
        public string Number { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("status_code")]
        public string StatusCode { get; set; }

        [JsonPropertyName("amount")]
        public int? Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("credit_terms")]
        public string CreditTerms { get; set; }

        [JsonPropertyName("paramj")]
        public int? ParamJ { get; set; }

        [JsonPropertyName("rrn")]
        public string Rrn { get; set; }

        [JsonPropertyName("payments")]
        public PaymentsWebhook Payments { get; set; }

        [JsonPropertyName("secure3D")]
        public Secure3D Secure3D { get; set; }

        [JsonPropertyName("approval_number")]
        public string ApprovalNumber { get; set; }

        [JsonPropertyName("voucher_number")]
        public string VoucherNumber { get; set; }

        [JsonPropertyName("more_info")]
        public string MoreInfo { get; set; }

        [JsonPropertyName("more_info_1")]
        public string MoreInfo1 { get; set; }

        [JsonPropertyName("more_info_2")]
        public string MoreInfo2 { get; set; }

        [JsonPropertyName("more_info_3")]
        public string MoreInfo3 { get; set; }

        [JsonPropertyName("more_info_4")]
        public string MoreInfo4 { get; set; }

        [JsonPropertyName("more_info_5")]
        public string MoreInfo5 { get; set; }

        [JsonPropertyName("add_data")]
        public string AddData { get; set; }

        [JsonPropertyName("original_amount_currency_dcc")]
        public string OriginalAmountCurrencyDcc { get; set; }
    }

    public class PaymentsWebhook
    {
        [JsonPropertyName("number_of_payments")]
        public int? NumberOfPayments { get; set; }

        [JsonPropertyName("first_payment_amount")]
        public int? FirstPaymentAmount { get; set; }

        [JsonPropertyName("rest_payments_amount")]
        public int? RestPaymentsAmount { get; set; }
    }

    public class Secure3DWebhook
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("tracking")]
        public string Tracking { get; set; }
    }

    public class DataBlock
    {
        [JsonPropertyName("customer_uid")]
        public string CustomerUid { get; set; }

        [JsonPropertyName("customer_email")]
        public string CustomerEmail { get; set; }

        [JsonPropertyName("terminal_uid")]
        public string TerminalUid { get; set; }

        [JsonPropertyName("cashier_uid")]
        public string CashierUid { get; set; }

        [JsonPropertyName("cashier_name")]
        public string CashierName { get; set; }

        [JsonPropertyName("items")]
        public List<Item> Items { get; set; }

        [JsonPropertyName("card_information")]
        public CardInformationWebhook CardInformation { get; set; }
    }

    public class Item
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("amount_pay")]
        public int? AmountPay { get; set; }

        [JsonPropertyName("discount_amount")]
        public int? DiscountAmount { get; set; }

        [JsonPropertyName("discount_type")]
        public string DiscountType { get; set; }

        [JsonPropertyName("discount_value")]
        public decimal? DiscountValue { get; set; }

        [JsonPropertyName("quantity")]
        public int? Quantity { get; set; }

        [JsonPropertyName("quantity_price")]
        public decimal? QuantityPrice { get; set; }

        [JsonPropertyName("product_uid")]
        public string ProductUid { get; set; }

        [JsonPropertyName("product_variant_uid")]
        public string ProductVariantUid { get; set; }

        [JsonPropertyName("vat")]
        public decimal? Vat { get; set; }

        [JsonPropertyName("barcode")]
        public string Barcode { get; set; }
    }

    public class CardInformationWebhook
    {
        [JsonPropertyName("card_bin")]
        public string CardBin { get; set; }

        [JsonPropertyName("card_holder_name")]
        public string CardHolderName { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("four_digits")]
        public string FourDigits { get; set; }

        [JsonPropertyName("expiry_month")]
        public string ExpiryMonth { get; set; }

        [JsonPropertyName("expiry_year")]
        public string ExpiryYear { get; set; }

        [JsonPropertyName("clearing_id")]
        public int? ClearingId { get; set; }

        [JsonPropertyName("brand_id")]
        public int? BrandId { get; set; }

        [JsonPropertyName("issuer_id")]
        public int? IssuerId { get; set; }

        [JsonPropertyName("card_foreign")]
        public string CardForeign { get; set; }

        [JsonPropertyName("identification_number")]
        public string IdentificationNumber { get; set; }

        [JsonPropertyName("token_number")]
        public string TokenNumber { get; set; }
    }

    public class InvoiceWebhook
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("docu_number")]
        public string DocuNumber { get; set; }

        [JsonPropertyName("original_url")]
        public string OriginalUrl { get; set; }

        [JsonPropertyName("copy_url")]
        public string CopyUrl { get; set; }

        [JsonPropertyName("integrator_name")]
        public string IntegratorName { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }

    public class GetDocumentsRequest
    {
        public string TransactionUid { get; set; }
        public string FromDate { get; set; }
        public string UntilDate { get; set; }
    }

    //****************************************************************************************** זיכוי לפי טרנזקציה
    public class TransactionRefund
    {
        [JsonPropertyName("transaction_uid")]
        public string TransactionUid { get; set; }

        [JsonPropertyName("amount")]
        public decimal? Amount { get; set; }

        [JsonPropertyName("more_info")]
        public string MoreInfo { get; set; }

        [JsonPropertyName("valid")]
        public bool Valid { get; set; }

        [JsonPropertyName("cvv")]
        public string Cvv { get; set; }

        [JsonPropertyName("initial_invoice")]
        public bool InitialInvoice { get; set; }

        [JsonPropertyName("items")]
        public List<TransactionRefundItem> Items { get; set; }

        public TransactionRefund()
        {
            Valid = true;
            InitialInvoice = true;
        }
    }

    public class TransactionRefundItem
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("product_uid")]
        public string ProductUid { get; set; }

        [JsonPropertyName("image_url")]
        public string ImageUrl { get; set; }

        [JsonPropertyName("category_uid")]
        public string CategoryUid { get; set; }

        [JsonPropertyName("quantity")]
        public string Quantity { get; set; }

        [JsonPropertyName("barcode")]
        public string Barcode { get; set; }

        [JsonPropertyName("value")]
        public int? Value { get; set; }

        [JsonPropertyName("price")]
        public int? Price { get; set; }

        [JsonPropertyName("discount_type")]
        public string DiscountType { get; set; }

        [JsonPropertyName("discount_value")]
        public int? DiscountValue { get; set; }

        [JsonPropertyName("shipping")]
        public bool? Shipping { get; set; }

        [JsonPropertyName("vat_type")]
        public string VatType { get; set; }

        public TransactionRefundItem()
        {
            DiscountType = "amount";
            DiscountValue = 0;
        }
    }

    //****************************************************************************************** תגובה לפי טרנזקציה
    public class TransactionRefundResponse
    {
        [JsonPropertyName("results")]
        public PayPlusResult Results { get; set; }

        [JsonPropertyName("data")]
        public PayPlusInternalPageChargeData Data { get; set; }
    }

    public class PayPlusInternalPageChargeData
    {
        [JsonPropertyName("transaction")]
        public PayPlusTransactionRefund Transaction { get; set; }

        [JsonPropertyName("data")]
        public PayPlusChargeInnerData InnerData { get; set; }
    }

    public class PayPlusTransactionRefund
    {
        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        [JsonPropertyName("number")]
        public string Number { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("status_code")]
        public string StatusCode { get; set; }

        [JsonPropertyName("amount")]
        public decimal? Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("credit_terms")]
        public string CreditTerms { get; set; }

        [JsonPropertyName("payments")]
        public ChargePayments Payments { get; set; }

        [JsonPropertyName("secure3D")]
        public ChargeSecure3D Secure3D { get; set; }

        [JsonPropertyName("approval_number")]
        public string ApprovalNumber { get; set; }

        [JsonPropertyName("voucher_number")]
        public string VoucherNumber { get; set; }

        [JsonPropertyName("more_info")]
        public string MoreInfo { get; set; }
    }

    public class ChargePayments
    {
        [JsonPropertyName("number_of_payments")]
        public int? NumberOfPayments { get; set; }

        [JsonPropertyName("first_payment_amount")]
        public decimal? FirstPaymentAmount { get; set; }

        [JsonPropertyName("rest_payments_amount")]
        public decimal? RestPaymentsAmount { get; set; }
    }

    public class ChargeSecure3D
    {
        [JsonPropertyName("status")]
        public bool? Status { get; set; }

        [JsonPropertyName("tracking")]
        public string Tracking { get; set; }
    }

    public class ChargeItem
    {
        [JsonPropertyName("amount_pay")]
        public decimal? AmountPay { get; set; }

        [JsonPropertyName("discount_amount")]
        public decimal? DiscountAmount { get; set; }

        [JsonPropertyName("quantity")]
        public int? Quantity { get; set; }

        [JsonPropertyName("quantity_price")]
        public decimal? QuantityPrice { get; set; }

        [JsonPropertyName("vat")]
        public decimal? Vat { get; set; }

        [JsonPropertyName("product_uid")]
        public string ProductUid { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class ChargeCardInformation
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("four_digits")]
        public string FourDigits { get; set; }

        [JsonPropertyName("expiry_month")]
        public string ExpiryMonth { get; set; }

        [JsonPropertyName("expiry_year")]
        public string ExpiryYear { get; set; }

        [JsonPropertyName("clearing_id")]
        public int? ClearingId { get; set; }

        [JsonPropertyName("brand_id")]
        public int? BrandId { get; set; }

        [JsonPropertyName("issuer_id")]
        public int? IssuerId { get; set; }
    }
}
