using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;

namespace EasyBank.BAL
{
    public enum ErrorCode
    {
        [Description("Everything is fine")]
        Ok = 0,
        [Description("Too small amount (min 5 for TA)")]
        TooSmallAmountForTA = 1,

        [Description("Too small amount (min 100 for DA)")]
        TooSmallAmountForDA = 2,

        [Description("Negative or zero operation amount")]
        NegativeOrZeroOperationAmount = 3,

        [Description("account is Blocked or Frozen or Expired")]
        AcountIsBlFrExp = 4,

        [Description("not enough money on account")]
        NotEnoughMoney = 5,

        [Description("attempt to widthdraw from credit account")]
        AttemptToWidthdrawFromCreditAcc = 6,

        [Description("Adding specified amount will make credit acc balance negative(can't be smaller then 0)")]
        AddingThisAmountWillMakeCredAccNegative = 7,

        [Description("Attempt to transfer from account that is not MT account")]
        AttemptToTransferFromNotMoneyTransferAcc = 8,

        [Description("Attempt to transfer to account that is not MT account")]
        AttemptToTransferToNotMoneyTransferAcc = 9,

        [Description("If early termination is false only whole amount can be widthdrawn")]
        IfEarlyTermFalseOnlyWholeAmountCanBeWidthdrawn = 31,

        [Description("Can't transfer money to yourself")]
        CantTransferMoneyToYourself = 32,

        [Description("Can't use closed account")]
        CantUseClosedAccount = 33,

        [Description("Operators email == null")]
        NullOperatorsEmail = 10,

        [Description("AccountId == null")]
        NullAccountId = 11,

        [Description("Amount == null")]
        NullAmount = 12,

        [Description("invalid source currency")]
        InvalidSourceCurrency = 13,

        [Description("invalid target currency")]
        InvalidTargetCurrency = 14,

        [Description("Operator not found in db")]
        OperatorNotFoundInDb = 21,

        [Description("Account not found in db")]
        AccountNotFoundInDb = 22,

        [Description("Source currency not found in db")]
        SourceCurrencyNotFoundInDb = 23,

        [Description("Target currency not found in db")]
        TargetCurrencyNotFoundInDb = 24,

        [Description("Source currency fail")]
        SourceCurrencyFail = 25,

        [Description("Target currency fail")]
        TargetCurrencyFail = 26,

        [Description("Sending account is blocked")]
        SendingAccountIsBlocked = 41,

        [Description("Sending account is frozen")]
        SendingAccountIsFrozen = 42,

        [Description("Sending account is expired")]
        SendingAccountIsExpired = 43,

        [Description("Receiving account is blocked")]//k,
        ReceivingAccountIsBlocked = 44,

        [Description("Receiving account is frozen")]
        ReceivingAccountIsFrozen = 45,

        [Description("Receiving account is expired")]
        ReceivingAccountIsExpired = 46,

        [Description("No bank receiving account with specified name")]
        NoBankReceivingAccountWithSpecifiedName = 50,

        [Description("No bank sending account with specified name")]
        NoBankSendingAccountWithSpecifiedName = 51,

        [Description("Not enough money on sending bank account")]
        NotEnoughMoneyOnSendingBankAccount = 55,

        [Description("Uncatched error")]
        UncatchedError = 99
    }

    public class ErrorHandler
    {
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}