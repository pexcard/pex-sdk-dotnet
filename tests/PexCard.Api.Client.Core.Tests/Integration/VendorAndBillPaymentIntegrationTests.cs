using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PexCard.Api.Client;
using PexCard.Api.Client.Core;
using PexCard.Api.Client.Core.Enums;
using PexCard.Api.Client.Core.Models;
using Xunit;
using Xunit.Abstractions;

namespace PexCard.Api.Client.Core.Tests.Integration
{
    /// <summary>
    /// End-to-end integration tests and documentation for PEX Bill Pay workflows.
    ///
    /// This test serves as living documentation demonstrating the complete flow
    /// for paying vendors via ACH using the PEX Bill Pay API.
    ///
    /// Prerequisites:
    /// - Configure appsettings.local.json with PexApi:Token set to a valid external token
    /// - Test business must have Bill Pay and Vendor Management enabled
    /// - Business must have ACH payments enabled
    ///
    /// Configuration:
    /// - appsettings.json contains defaults (committed to repo)
    /// - appsettings.local.json contains your credentials (not committed, add to .gitignore)
    /// </summary>
    [Trait("Category", "Integration")]
    public class VendorAndBillPaymentIntegrationTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IPexApiClient _pexApiClient;
        private readonly string _apiToken;

        public VendorAndBillPaymentIntegrationTests(ITestOutputHelper output)
        {
            _output = output;

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.local.json", optional: true)
                .Build();

            var apiBaseUrl = configuration["PexApi:BaseUrl"] ?? "https://coreapi.pexcard.com";
            _apiToken = configuration["PexApi:Token"];

            if (string.IsNullOrEmpty(_apiToken) || _apiToken == "YOUR_TOKEN_HERE")
            {
                _output.WriteLine("WARNING: PexApi:Token not configured in appsettings.local.json. Integration tests will be skipped.");
                _apiToken = null;
            }

            var httpClient = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
            _pexApiClient = new PexApiClient(httpClient);
        }

        /// <summary>
        /// Complete Bill Pay workflow demonstration.
        /// This test shows how to pay a vendor via ACH from start to finish.
        /// </summary>
        [Fact]
        public async Task EndToEnd_VendorAndBillPayment_CompletesFullWorkflow()
        {
            // Skip if no token configured
            if (string.IsNullOrEmpty(_apiToken))
            {
                _output.WriteLine("Skipping test - PexApi:Token not configured in appsettings.local.json");
                return;
            }

            var testAmount = 100.00m + (decimal)new Random().Next(1, 99) / 100;
            var vendorName = $"SDK-Test-Vendor-{DateTime.Now:yyyyMMdd-HHmmss}";
            var billRefNo = $"SDK-BILL-{DateTime.Now:yyyyMMdd-HHmmss}-{new Random().Next(1000, 9999)}";

            PrintHeader();
            PrintWorkflowOverview();
            await PrintBusinessContext();

            // ═══════════════════════════════════════════════════════════════════════════════════════
            // PHASE 1: VENDOR SETUP
            // Before you can pay a vendor, you must create and onboard them in the system.
            // ═══════════════════════════════════════════════════════════════════════════════════════

            PrintPhaseHeader("PHASE 1: VENDOR SETUP", "Before paying a vendor, they must be created and onboarded");

            // Step 1: Create Vendor
            var vendor = await Step1_CreateVendor(vendorName);

            // Step 2: Add Bank Account
            vendor = await Step2_AddBankAccount(vendor.VendorId);

            // Step 3: Approve Vendor (if required by business policy)
            vendor = await Step3_ApproveVendor(vendor);

            // ═══════════════════════════════════════════════════════════════════════════════════════
            // PHASE 2: BILL CREATION
            // Create a bill (payment request) for the vendor.
            // ═══════════════════════════════════════════════════════════════════════════════════════

            PrintPhaseHeader("PHASE 2: BILL CREATION", "Create a payment request for the vendor");

            // Step 4: Create Bill
            var bill = await Step4_CreateBill(vendor.VendorId, testAmount, billRefNo);

            // ═══════════════════════════════════════════════════════════════════════════════════════
            // PHASE 3: APPROVAL WORKFLOW
            // Bills go through an approval process before payment.
            // The workflow depends on your business configuration.
            // ═══════════════════════════════════════════════════════════════════════════════════════

            PrintPhaseHeader("PHASE 3: APPROVAL WORKFLOW", "Bills must be approved before payment processing");

            // Step 5: Submit Bill for Approval
            bill = await Step5_SubmitBill(bill.BillId);

            // Step 6: Approve Bill
            bill = await Step6_ApproveBill(bill);

            // ═══════════════════════════════════════════════════════════════════════════════════════
            // PHASE 4: PAYMENT PROCESSING
            // Process the approved bill to initiate the ACH transfer.
            // ═══════════════════════════════════════════════════════════════════════════════════════

            PrintPhaseHeader("PHASE 4: PAYMENT PROCESSING", "Initiate the ACH transfer to the vendor");

            // Step 7: Process Payment
            bill = await Step7_ProcessBill(bill.BillId);

            // Step 8: Verify Payment
            await Step8_VerifyPayment(bill.BillId);

            // ═══════════════════════════════════════════════════════════════════════════════════════
            // COMPLETE
            // ═══════════════════════════════════════════════════════════════════════════════════════

            PrintCompletion(vendor.VendorId, bill.BillId, testAmount, bill.PaymentRequestStatus);
        }

        #region Workflow Steps

        private async Task<VendorModel> Step1_CreateVendor(string vendorName)
        {
            _output.WriteLine("");
            _output.WriteLine("┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓");
            _output.WriteLine("┃  STEP 1: CREATE VENDOR                                                          ┃");
            _output.WriteLine("┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛");
            _output.WriteLine("");
            _output.WriteLine("  PURPOSE: Register a new vendor in the system before you can pay them.");
            _output.WriteLine("");
            _output.WriteLine("  ┌─ API DETAILS ────────────────────────────────────────────────────────────────┐");
            _output.WriteLine("  │  Method:   POST                                                              │");
            _output.WriteLine("  │  Endpoint: /V4/Vendor                                                        │");
            _output.WriteLine("  │                                                                              │");
            _output.WriteLine("  │  Required Fields:                                                            │");
            _output.WriteLine("  │    • VendorName          - Display name for the vendor                       │");
            _output.WriteLine("  │    • EmailForRemittance  - Email to send payment notifications               │");
            _output.WriteLine("  │                                                                              │");
            _output.WriteLine("  │  Payment Method (at least one required):                                     │");
            _output.WriteLine("  │    • AchPaymentEnabled: true      - Enable ACH bank transfers                │");
            _output.WriteLine("  │    • VendorCardPaymentEnabled: true - Enable virtual card payments           │");
            _output.WriteLine("  │                                                                              │");
            _output.WriteLine("  │  Optional Fields:                                                            │");
            _output.WriteLine("  │    • VendorAddress      - Vendor's mailing address                           │");
            _output.WriteLine("  │    • VendorContact      - Primary contact person                             │");
            _output.WriteLine("  │    • SendNotification   - Email vendor when payments are made                │");
            _output.WriteLine("  └──────────────────────────────────────────────────────────────────────────────┘");
            _output.WriteLine("");

            var createVendorRequest = new CreateVendorRequestModel
            {
                VendorName = vendorName,
                EmailForRemittance = "test@example.com",
                AchPaymentEnabled = true,
                VendorCardPaymentEnabled = false,
                SendNotification = false,
                VendorAddress = new VendorAddressModel
                {
                    AddressLine1 = "123 Test Street",
                    City = "Test City",
                    State = "NY",
                    PostalCode = "10001"
                },
                VendorContact = new VendorContactModel
                {
                    FirstName = "Test",
                    LastName = "Contact",
                    Email = "contact@example.com"
                }
            };

            _output.WriteLine("  ⏳ Creating vendor...");
            var vendor = await _pexApiClient.CreateVendor(_apiToken, createVendorRequest);

            _output.WriteLine($"  ✅ Vendor created successfully!");
            _output.WriteLine("");
            PrintVendorState(vendor);
            PrintVendorStateMachine(vendor.VendorStatus, vendor.VendorStatusTrigger);

            Assert.True(vendor.VendorId > 0);
            return vendor;
        }

        private async Task<VendorModel> Step2_AddBankAccount(int vendorId)
        {
            _output.WriteLine("");
            _output.WriteLine("┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓");
            _output.WriteLine("┃  STEP 2: ADD BANK ACCOUNT                                                       ┃");
            _output.WriteLine("┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛");
            _output.WriteLine("");
            _output.WriteLine("  PURPOSE: Add ACH bank account details so payments can be sent to the vendor.");
            _output.WriteLine("");
            _output.WriteLine("  ┌─ API DETAILS ────────────────────────────────────────────────────────────────┐");
            _output.WriteLine("  │  Method:   POST                                                              │");
            _output.WriteLine("  │  Endpoint: /V4/Vendor/{vendorId}/BankAccount                                 │");
            _output.WriteLine("  │                                                                              │");
            _output.WriteLine("  │  Required Fields:                                                            │");
            _output.WriteLine("  │    • BankName           - Name of the bank                                   │");
            _output.WriteLine("  │    • BankRoutingNumber  - 9-digit ABA routing number                         │");
            _output.WriteLine("  │    • BankAccountNumber  - Account number                                     │");
            _output.WriteLine("  │                                                                              │");
            _output.WriteLine("  │  Note: Bank account must be added before ACH payments can be processed.      │");
            _output.WriteLine("  └──────────────────────────────────────────────────────────────────────────────┘");
            _output.WriteLine("");

            var addBankRequest = new AddVendorBankAccountRequestModel
            {
                BankName = "Test Bank",
                BankRoutingNumber = "021000021",
                BankAccountNumber = "123456789"
            };

            _output.WriteLine("  ⏳ Adding bank account...");
            var vendor = await _pexApiClient.AddVendorBankAccount(_apiToken, vendorId, addBankRequest);

            _output.WriteLine($"  ✅ Bank account added successfully!");
            _output.WriteLine($"     Bank Accounts: {vendor.BankAccounts?.Count ?? 0}");
            _output.WriteLine("");

            return vendor;
        }

        private async Task<VendorModel> Step3_ApproveVendor(VendorModel vendor)
        {
            _output.WriteLine("");
            _output.WriteLine("┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓");
            _output.WriteLine("┃  STEP 3: APPROVE VENDOR                                                         ┃");
            _output.WriteLine("┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛");
            _output.WriteLine("");
            _output.WriteLine("  PURPOSE: Activate the vendor so they can receive payments.");
            _output.WriteLine("           Required only if your business has an Approval Policy workflow configured.");
            _output.WriteLine("");
            _output.WriteLine("  ┌─ API DETAILS ────────────────────────────────────────────────────────────────┐");
            _output.WriteLine("  │  Method:   POST                                                              │");
            _output.WriteLine("  │  Endpoint: /V4/Vendor/{vendorId}/Approve                                     │");
            _output.WriteLine("  │                                                                              │");
            _output.WriteLine("  │  No request body required.                                                   │");
            _output.WriteLine("  │                                                                              │");
            _output.WriteLine("  │  Prerequisites:                                                              │");
            _output.WriteLine("  │    • Vendor must exist                                                       │");
            _output.WriteLine("  │    • User must have vendor approval permissions                              │");
            _output.WriteLine("  │                                                                              │");
            _output.WriteLine("  │  Result: Vendor status changes to Onboarded/Active                           │");
            _output.WriteLine("  └──────────────────────────────────────────────────────────────────────────────┘");
            _output.WriteLine("");

            if (vendor.VendorStatus == VendorStatus.Onboarded)
            {
                _output.WriteLine("  ℹ️  Vendor already onboarded (no Approval Policy workflow configured).");
                _output.WriteLine("");
                PrintVendorState(vendor);
                return vendor;
            }

            try
            {
                _output.WriteLine("  ⏳ Approving vendor...");
                vendor = await _pexApiClient.ApproveVendor(_apiToken, vendor.VendorId);
                _output.WriteLine($"  ✅ Vendor approved successfully!");
            }
            catch (PexCard.Api.Client.Core.Exceptions.PexApiClientException ex)
            {
                _output.WriteLine($"  ⚠️  Could not approve vendor: {ex.Message}");
                vendor = await _pexApiClient.GetVendor(_apiToken, vendor.VendorId);

                if (vendor.VendorStatus != VendorStatus.Onboarded)
                {
                    _output.WriteLine("");
                    _output.WriteLine("  ❌ STOPPING: Vendor must be Onboarded to create bills.");
                    Assert.Fail($"Vendor approval required but failed: {ex.Message}");
                }
            }

            _output.WriteLine("");
            PrintVendorState(vendor);
            PrintVendorStateMachine(vendor.VendorStatus, vendor.VendorStatusTrigger);

            return vendor;
        }

        private async Task<BillModel> Step4_CreateBill(int vendorId, decimal amount, string billRefNo)
        {
            _output.WriteLine("");
            _output.WriteLine("┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓");
            _output.WriteLine("┃  STEP 4: CREATE BILL                                                            ┃");
            _output.WriteLine("┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛");
            _output.WriteLine("");
            _output.WriteLine("  PURPOSE: Create a bill (payment request) to pay the vendor.");
            _output.WriteLine("           A bill represents a payment you want to make to a vendor.");
            _output.WriteLine("");
            _output.WriteLine("  ┌─ API DETAILS ────────────────────────────────────────────────────────────────┐");
            _output.WriteLine("  │  Method:   POST                                                              │");
            _output.WriteLine("  │  Endpoint: /V4/Bill                                                          │");
            _output.WriteLine("  │                                                                              │");
            _output.WriteLine("  │  Required Fields:                                                            │");
            _output.WriteLine("  │    • VendorId       - ID of the vendor to pay                                │");
            _output.WriteLine("  │    • Amount         - Payment amount in USD                                  │");
            _output.WriteLine("  │    • PaymentMethod  - ACH or VendorCard                                      │");
            _output.WriteLine("  │                                                                              │");
            _output.WriteLine("  │  Optional Fields:                                                            │");
            _output.WriteLine("  │    • BillPayment.BillDate    - Date of the invoice/bill                      │");
            _output.WriteLine("  │    • BillPayment.DueDate     - When payment is due                           │");
            _output.WriteLine("  │    • BillPayment.BillRefNo   - Your reference number for tracking            │");
            _output.WriteLine("  │                                                                              │");
            _output.WriteLine("  │  Result: Bill created in Draft status                                        │");
            _output.WriteLine("  └──────────────────────────────────────────────────────────────────────────────┘");
            _output.WriteLine("");

            var createBillRequest = new CreateBillRequestModel
            {
                VendorId = vendorId,
                Amount = amount,
                PaymentMethod = BillPaymentMethodType.ACH,
                BillPayment = new BillPaymentDetailsModel
                {
                    BillDate = DateTimeOffset.Now.AddDays(-5),
                    DueDate = DateTimeOffset.Now.AddDays(25),
                    BillRefNo = billRefNo
                }
            };

            _output.WriteLine("  ⏳ Creating bill...");
            var bill = await _pexApiClient.CreateBill(_apiToken, createBillRequest);

            _output.WriteLine($"  ✅ Bill created successfully!");
            _output.WriteLine("");
            PrintBillState(bill);
            PrintBillStateMachine(bill.PaymentRequestStatus);

            Assert.True(bill.BillId > 0);
            Assert.Equal(amount, bill.Amount);
            return bill;
        }

        private async Task<BillModel> Step5_SubmitBill(int billId)
        {
            _output.WriteLine("");
            _output.WriteLine("┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓");
            _output.WriteLine("┃  STEP 5: SUBMIT BILL FOR APPROVAL                                               ┃");
            _output.WriteLine("┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛");
            _output.WriteLine("");
            _output.WriteLine("  PURPOSE: Submit the bill for approval workflow processing.");
            _output.WriteLine("           After submission, the bill enters your business's approval workflow.");
            _output.WriteLine("");
            _output.WriteLine("  ┌─ API DETAILS ────────────────────────────────────────────────────────────────┐");
            _output.WriteLine("  │  Method:   POST                                                              │");
            _output.WriteLine("  │  Endpoint: /V4/Bill/{billId}/Submit                                          │");
            _output.WriteLine("  │                                                                              │");
            _output.WriteLine("  │  No request body required.                                                   │");
            _output.WriteLine("  │                                                                              │");
            _output.WriteLine("  │  Prerequisites:                                                              │");
            _output.WriteLine("  │    • Bill must be in Draft status                                            │");
            _output.WriteLine("  │                                                                              │");
            _output.WriteLine("  │  Result: Bill moves to Pending/Submitted status                              │");
            _output.WriteLine("  └──────────────────────────────────────────────────────────────────────────────┘");
            _output.WriteLine("");

            _output.WriteLine("  ⏳ Submitting bill for approval...");
            var bill = await _pexApiClient.SubmitBill(_apiToken, billId);

            _output.WriteLine($"  ✅ Bill submitted successfully!");
            _output.WriteLine("");
            PrintBillState(bill);
            PrintBillStateMachine(bill.PaymentRequestStatus);

            return bill;
        }

        private async Task<BillModel> Step6_ApproveBill(BillModel bill)
        {
            _output.WriteLine("");
            _output.WriteLine("┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓");
            _output.WriteLine("┃  STEP 6: APPROVE BILL                                                           ┃");
            _output.WriteLine("┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛");
            _output.WriteLine("");
            _output.WriteLine("  PURPOSE: Approve the bill for payment processing.");
            _output.WriteLine("           Required only if your business has an Approval Policy workflow configured.");
            _output.WriteLine("");
            _output.WriteLine("  ┌─ API DETAILS ────────────────────────────────────────────────────────────────┐");
            _output.WriteLine("  │  Method:   POST                                                              │");
            _output.WriteLine("  │  Endpoint: /V4/Bill/{billId}/Approve                                         │");
            _output.WriteLine("  │                                                                              │");
            _output.WriteLine("  │  Optional Fields:                                                            │");
            _output.WriteLine("  │    • Reason  - Approval reason/notes                                         │");
            _output.WriteLine("  │                                                                              │");
            _output.WriteLine("  │  Prerequisites:                                                              │");
            _output.WriteLine("  │    • Bill must be in Pending/Submitted status                                │");
            _output.WriteLine("  │    • User must have approval permissions                                     │");
            _output.WriteLine("  │                                                                              │");
            _output.WriteLine("  │  Result: Bill moves to Approved status, ready for processing                 │");
            _output.WriteLine("  └──────────────────────────────────────────────────────────────────────────────┘");
            _output.WriteLine("");

            if (bill.PaymentRequestStatus == "Approved" || bill.PaymentRequestStatus == "PendingPaymentTransfer")
            {
                _output.WriteLine("  ℹ️  Bill already approved (no Approval Policy workflow configured).");
                _output.WriteLine("");
                PrintBillState(bill);
                return bill;
            }

            _output.WriteLine("  ⏳ Approving bill...");
            bill = await _pexApiClient.ApproveBill(_apiToken, bill.BillId, new ApproveBillRequestModel
            {
                Reason = "SDK Integration Test approval"
            });

            _output.WriteLine($"  ✅ Bill approved successfully!");
            _output.WriteLine("");
            PrintBillState(bill);
            PrintBillStateMachine(bill.PaymentRequestStatus);

            return bill;
        }

        private async Task<BillModel> Step7_ProcessBill(int billId)
        {
            _output.WriteLine("");
            _output.WriteLine("┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓");
            _output.WriteLine("┃  STEP 7: PROCESS PAYMENT                                                        ┃");
            _output.WriteLine("┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛");
            _output.WriteLine("");
            _output.WriteLine("  PURPOSE: Initiate the actual ACH transfer to the vendor's bank account.");
            _output.WriteLine("           This step sends the payment to the ACH network for processing.");
            _output.WriteLine("");
            _output.WriteLine("  ┌─ API DETAILS ────────────────────────────────────────────────────────────────┐");
            _output.WriteLine("  │  Method:   POST                                                              │");
            _output.WriteLine("  │  Endpoint: /V4/Bill/{billId}/Process                                         │");
            _output.WriteLine("  │                                                                              │");
            _output.WriteLine("  │  Optional Fields:                                                            │");
            _output.WriteLine("  │    • ProcessingDate  - Date to process (defaults to today)                   │");
            _output.WriteLine("  │                                                                              │");
            _output.WriteLine("  │  Prerequisites:                                                              │");
            _output.WriteLine("  │    • Bill must be Approved                                                   │");
            _output.WriteLine("  │    • Vendor must have valid bank account                                     │");
            _output.WriteLine("  │    • Business must have sufficient balance                                   │");
            _output.WriteLine("  │                                                                              │");
            _output.WriteLine("  │  Result: ACH transfer initiated, bill moves to Processing status             │");
            _output.WriteLine("  │                                                                              │");
            _output.WriteLine("  │  Note: ACH transfers typically take 1-3 business days to complete.           │");
            _output.WriteLine("  └──────────────────────────────────────────────────────────────────────────────┘");
            _output.WriteLine("");

            // Refresh bill state before processing
            var bill = await _pexApiClient.GetBill(_apiToken, billId);
            _output.WriteLine($"  📋 Bill state before processing:");
            _output.WriteLine($"     Status: {bill.PaymentRequestStatus} / {bill.PaymentRequestStatusTrigger}");
            _output.WriteLine("");

            _output.WriteLine("  ⏳ Processing payment...");
            try
            {
                bill = await _pexApiClient.ProcessBill(_apiToken, billId);
                _output.WriteLine($"  ✅ Payment processed successfully!");
            }
            catch (PexCard.Api.Client.Core.Exceptions.PexApiClientException ex)
            {
                _output.WriteLine($"  ❌ FAILED: {ex.Message}");
                _output.WriteLine($"     HTTP Status: {(int)ex.Code} {ex.Code}");
                _output.WriteLine("");
                _output.WriteLine("  Common reasons for failure:");
                _output.WriteLine("    • Bill not in Approved status");
                _output.WriteLine("    • Vendor bank account not valid");
                _output.WriteLine("    • Insufficient business balance");
                _output.WriteLine("    • ACH processing not enabled");
                throw;
            }

            _output.WriteLine("");
            PrintBillState(bill);
            PrintBillStateMachine(bill.PaymentRequestStatus);

            return bill;
        }

        private async Task Step8_VerifyPayment(int billId)
        {
            _output.WriteLine("");
            _output.WriteLine("┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓");
            _output.WriteLine("┃  STEP 8: VERIFY PAYMENT                                                         ┃");
            _output.WriteLine("┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛");
            _output.WriteLine("");
            _output.WriteLine("  PURPOSE: Retrieve payment records to verify the ACH transfer was initiated.");
            _output.WriteLine("");
            _output.WriteLine("  ┌─ API DETAILS ────────────────────────────────────────────────────────────────┐");
            _output.WriteLine("  │  Method:   GET                                                               │");
            _output.WriteLine("  │  Endpoint: /V4/Bill/{billId}/Payments                                        │");
            _output.WriteLine("  │                                                                              │");
            _output.WriteLine("  │  Returns: List of payment records associated with the bill                   │");
            _output.WriteLine("  │                                                                              │");
            _output.WriteLine("  │  Payment Status Values:                                                      │");
            _output.WriteLine("  │    • Pending     - Payment created, not yet sent                             │");
            _output.WriteLine("  │    • Processing  - Sent to ACH network                                       │");
            _output.WriteLine("  │    • Completed   - Successfully delivered                                    │");
            _output.WriteLine("  │    • Failed      - Transfer failed (NSF, invalid account, etc.)              │");
            _output.WriteLine("  │    • Returned    - Returned by receiving bank                                │");
            _output.WriteLine("  └──────────────────────────────────────────────────────────────────────────────┘");
            _output.WriteLine("");

            _output.WriteLine("  ⏳ Fetching payment records...");
            var payments = await _pexApiClient.GetBillPayments(_apiToken, billId);

            _output.WriteLine($"  ✅ Payment records retrieved!");
            _output.WriteLine("");
            _output.WriteLine($"  Payment Count: {payments.Payments?.Count ?? 0}");
            _output.WriteLine("");

            if (payments.Payments != null && payments.Payments.Count > 0)
            {
                _output.WriteLine("  ┌─ PAYMENT RECORDS ────────────────────────────────────────────────────────────┐");
                foreach (var payment in payments.Payments)
                {
                    _output.WriteLine($"  │  Payment ID: {payment.PaymentId,-20} Amount: ${payment.Amount,-15} Status: {payment.PaymentStatus,-10} │");
                }
                _output.WriteLine("  └──────────────────────────────────────────────────────────────────────────────┘");
            }
            _output.WriteLine("");
        }

        #endregion

        #region Output Helpers

        private void PrintHeader()
        {
            _output.WriteLine("");
            _output.WriteLine("╔══════════════════════════════════════════════════════════════════════════════════╗");
            _output.WriteLine("║                                                                                  ║");
            _output.WriteLine("║   PEX BILL PAY API - COMPLETE WORKFLOW DEMONSTRATION                             ║");
            _output.WriteLine("║                                                                                  ║");
            _output.WriteLine("║   This test demonstrates the full flow for paying vendors via ACH.               ║");
            _output.WriteLine("║   Use this as a reference for implementing Bill Pay in your application.         ║");
            _output.WriteLine("║                                                                                  ║");
            _output.WriteLine("╚══════════════════════════════════════════════════════════════════════════════════╝");
            _output.WriteLine("");
        }

        private void PrintWorkflowOverview()
        {
            _output.WriteLine("┌──────────────────────────────────────────────────────────────────────────────────┐");
            _output.WriteLine("│  WORKFLOW OVERVIEW                                                               │");
            _output.WriteLine("├──────────────────────────────────────────────────────────────────────────────────┤");
            _output.WriteLine("│                                                                                  │");
            _output.WriteLine("│  PHASE 1: VENDOR SETUP (One-time per vendor)                                     │");
            _output.WriteLine("│  ┌─────────────┐    ┌─────────────┐    ┌─────────────┐                           │");
            _output.WriteLine("│  │ 1. Create   │───►│ 2. Add Bank │───►│ 3. Approve  │                           │");
            _output.WriteLine("│  │    Vendor   │    │    Account  │    │    Vendor*  │                           │");
            _output.WriteLine("│  └─────────────┘    └─────────────┘    └─────────────┘                           │");
            _output.WriteLine("│                                               │                                  │");
            _output.WriteLine("│                                               ▼                                  │");
            _output.WriteLine("│  PHASE 2: CREATE BILL                   PHASE 3: APPROVAL WORKFLOW               │");
            _output.WriteLine("│  ┌─────────────┐                        ┌─────────────┐    ┌─────────────┐       │");
            _output.WriteLine("│  │ 4. Create   │───────────────────────►│ 5. Submit   │───►│ 6. Approve  │       │");
            _output.WriteLine("│  │    Bill     │                        │    Bill     │    │    Bill*    │       │");
            _output.WriteLine("│  └─────────────┘                        └─────────────┘    └─────────────┘       │");
            _output.WriteLine("│                                                                   │              │");
            _output.WriteLine("│                                                                   ▼              │");
            _output.WriteLine("│  PHASE 4: PAYMENT PROCESSING                                                     │");
            _output.WriteLine("│  ┌─────────────┐    ┌─────────────┐                                              │");
            _output.WriteLine("│  │ 7. Process  │───►│ 8. Verify   │     * Required only if business has an       │");
            _output.WriteLine("│  │    Payment  │    │    Payment  │       Approval Policy workflow configured    │");
            _output.WriteLine("│  └─────────────┘    └─────────────┘                                              │");
            _output.WriteLine("│        │                                                                         │");
            _output.WriteLine("│        └───────────► ACH transfer sent to vendor's bank (1-3 business days)      │");
            _output.WriteLine("│                                                                                  │");
            _output.WriteLine("│  Repeat steps 4-8 for each bill payment to the same vendor.                      │");
            _output.WriteLine("│                                                                                  │");
            _output.WriteLine("└──────────────────────────────────────────────────────────────────────────────────┘");
            _output.WriteLine("");
        }

        private async Task PrintBusinessContext()
        {
            _output.WriteLine("┌──────────────────────────────────────────────────────────────────────────────────┐");
            _output.WriteLine("│  AUTHENTICATION & BUSINESS CONTEXT                                               │");
            _output.WriteLine("└──────────────────────────────────────────────────────────────────────────────────┘");
            _output.WriteLine("");

            var tokenData = await _pexApiClient.GetToken(_apiToken);
            var businessDetails = await _pexApiClient.GetBusinessDetails(_apiToken);

            _output.WriteLine("  ┌─ TOKEN ──────────────────────────────────────────────────────────────────────┐");
            _output.WriteLine($"  │  App ID:        {tokenData.AppId,-60} │");
            _output.WriteLine($"  │  Platform ID:   {tokenData.PlatformAccountId?.ToString() ?? "(null)",-60} │");
            _output.WriteLine($"  │  User Type:     {tokenData.UserType,-60} │");
            _output.WriteLine($"  │  Expires:       {tokenData.TokenExpiration:yyyy-MM-dd HH:mm:ss,-49} │");
            _output.WriteLine("  └──────────────────────────────────────────────────────────────────────────────┘");
            _output.WriteLine("");
            _output.WriteLine("  ┌─ BUSINESS ───────────────────────────────────────────────────────────────────┐");
            _output.WriteLine($"  │  ID:            {businessDetails.BusinessAccountId,-60} │");
            _output.WriteLine($"  │  Name:          {businessDetails.BusinessName,-60} │");
            _output.WriteLine($"  │  Status:        {businessDetails.BusinessAccountStatus,-60} │");
            _output.WriteLine($"  │  Balance:       ${businessDetails.BusinessAccountBalance,-59} │");
            _output.WriteLine("  └──────────────────────────────────────────────────────────────────────────────┘");
            _output.WriteLine("");
        }

        private void PrintPhaseHeader(string phase, string description)
        {
            _output.WriteLine("");
            _output.WriteLine("══════════════════════════════════════════════════════════════════════════════════");
            _output.WriteLine($"  {phase}");
            _output.WriteLine($"  {description}");
            _output.WriteLine("══════════════════════════════════════════════════════════════════════════════════");
        }

        private void PrintVendorState(VendorModel vendor)
        {
            _output.WriteLine("  ┌─ VENDOR STATE ───────────────────────────────────────────────────────────────┐");
            _output.WriteLine($"  │  Vendor ID:     {vendor.VendorId,-60} │");
            _output.WriteLine($"  │  Name:          {vendor.VendorName,-60} │");
            _output.WriteLine($"  │  Status:        {vendor.VendorStatus,-60} │");
            _output.WriteLine($"  │  Status Trigger:{vendor.VendorStatusTrigger,-60} │");
            _output.WriteLine($"  │  Bank Accounts: {vendor.BankAccounts?.Count ?? 0,-60} │");
            _output.WriteLine("  └──────────────────────────────────────────────────────────────────────────────┘");
            _output.WriteLine("");
        }

        private void PrintVendorStateMachine(VendorStatus currentStatus, VendorStatusTrigger trigger)
        {
            _output.WriteLine("  ┌─ VENDOR STATE MACHINE ───────────────────────────────────────────────────────┐");
            _output.WriteLine("  │                                                                              │");
            var draftMarker = currentStatus == VendorStatus.Draft ? ">>>" : "   ";
            var pendingMarker = currentStatus == VendorStatus.Pending ? ">>>" : "   ";
            var onboardedMarker = currentStatus == VendorStatus.Onboarded ? ">>>" : "   ";
            _output.WriteLine($"  │  {draftMarker} [Draft] ──── Submit ────► [Pending] ──── Approve ────► [Onboarded] {onboardedMarker}│");
            _output.WriteLine("  │                               │                                              │");
            _output.WriteLine("  │                               └──── Reject ────► [Closed]                    │");
            _output.WriteLine("  │                                                                              │");
            _output.WriteLine($"  │  Current: {currentStatus} / {trigger,-53} │");
            _output.WriteLine("  └──────────────────────────────────────────────────────────────────────────────┘");
            _output.WriteLine("");
        }

        private void PrintBillState(BillModel bill)
        {
            _output.WriteLine("  ┌─ BILL STATE ─────────────────────────────────────────────────────────────────┐");
            _output.WriteLine($"  │  Bill ID:       {bill.BillId,-60} │");
            _output.WriteLine($"  │  Amount:        ${bill.Amount,-59} │");
            _output.WriteLine($"  │  Status:        {bill.PaymentRequestStatus,-60} │");
            _output.WriteLine($"  │  Status Trigger:{bill.PaymentRequestStatusTrigger,-60} │");
            _output.WriteLine($"  │  Merchant:      {bill.MerchantName ?? "(not set)",-60} │");
            _output.WriteLine("  └──────────────────────────────────────────────────────────────────────────────┘");
            _output.WriteLine("");
        }

        private void PrintBillStateMachine(string currentStatus)
        {
            _output.WriteLine("  ┌─ BILL STATE MACHINE ─────────────────────────────────────────────────────────┐");
            _output.WriteLine("  │                                                                              │");
            _output.WriteLine("  │  [Draft] ──► [Submitted] ──► [Approved] ──► [Processing] ──► [Completed]     │");
            _output.WriteLine("  │     │             │              │               │                           │");
            _output.WriteLine("  │     │             └── Reject ──► [Rejected]      └── Fail ──► [Failed]       │");
            _output.WriteLine("  │     │                                                                        │");
            _output.WriteLine("  │     └── Cancel ──► [Cancelled]                                               │");
            _output.WriteLine("  │                                                                              │");
            _output.WriteLine($"  │  Current: {currentStatus,-65} │");
            _output.WriteLine("  └──────────────────────────────────────────────────────────────────────────────┘");
            _output.WriteLine("");
        }

        private void PrintCompletion(int vendorId, int billId, decimal amount, string finalStatus)
        {
            _output.WriteLine("");
            _output.WriteLine("╔══════════════════════════════════════════════════════════════════════════════════╗");
            _output.WriteLine("║                                                                                  ║");
            _output.WriteLine("║   ✅  BILL PAY WORKFLOW COMPLETED SUCCESSFULLY!                                  ║");
            _output.WriteLine("║                                                                                  ║");
            _output.WriteLine("╠══════════════════════════════════════════════════════════════════════════════════╣");
            _output.WriteLine("║                                                                                  ║");
            _output.WriteLine($"║   Vendor ID:     {vendorId,-61} ║");
            _output.WriteLine($"║   Bill ID:       {billId,-61} ║");
            _output.WriteLine($"║   Amount:        ${amount,-60} ║");
            _output.WriteLine($"║   Final Status:  {finalStatus,-61} ║");
            _output.WriteLine("║                                                                                  ║");
            _output.WriteLine("╠══════════════════════════════════════════════════════════════════════════════════╣");
            _output.WriteLine("║                                                                                  ║");
            _output.WriteLine("║   NEXT STEPS:                                                                    ║");
            _output.WriteLine("║   • ACH transfers typically complete in 1-3 business days                        ║");
            _output.WriteLine("║   • Use GET /V4/Bill/{billId}/Payments to check payment status                   ║");
            _output.WriteLine("║   • Set up webhooks to receive real-time payment status updates                  ║");
            _output.WriteLine("║                                                                                  ║");
            _output.WriteLine("║   RESOURCES:                                                                     ║");
            _output.WriteLine("║   • API Documentation: https://developer.pexcard.com                             ║");
            _output.WriteLine("║   • SDK Repository: https://github.com/pexcard/pex-sdk-dotnet                    ║");
            _output.WriteLine("║                                                                                  ║");
            _output.WriteLine("╚══════════════════════════════════════════════════════════════════════════════════╝");
            _output.WriteLine("");
        }

        #endregion
    }
}
