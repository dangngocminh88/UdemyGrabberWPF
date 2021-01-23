using System.Collections.Generic;

namespace UdemyGrabberWPF.Models
{
    public class CheckoutSubmitRequest
    {
        public string checkout_event { get; set; }
        public PaymentInfo payment_info { get; set; }
        public ShoppingCart shopping_cart { get; set; }
        public CheckoutSubmitRequest()
        {
            checkout_event = "Submit";
            shopping_cart = new ShoppingCart();
            payment_info = new PaymentInfo();
        }
    }
    public class PaymentInfo
    {
        public string payment_method { get; set; }
        public string payment_vendor { get; set; }
        public PaymentInfo()
        {
            payment_method = "free-method";
            payment_vendor = "Free";
        }
    }
    public class ShoppingCart
    {
        public bool is_cart { get; set; }
        public List<Item> items { get; set; }
        public ShoppingCart()
        {
            is_cart = false;
            items = new List<Item>();
            items.Add(new Item());
        }
    }
    public class Item
    {
        public BuyableContext buyableContext { get; set; }
        public long buyableId { get; set; }
        public string buyableType { get; set; }
        public DiscountInfo discountInfo { get; set; }
        public PurchasePrice purchasePrice { get; set; }
        public Item()
        {
            buyableContext = new BuyableContext();
            buyableId = 0;
            buyableType = "course";
            discountInfo = new DiscountInfo();
            purchasePrice = new PurchasePrice();
        }
    }
    public class BuyableContext
    {
        //public string contentLocaleId {get;set;}
        public BuyableContext()
        {
            //contentLocaleId = null;
        }
    }
    public class DiscountInfo
    {
        public string code { get; set; }
        public DiscountInfo()
        {
            code = "";
        }
    }
    public class PurchasePrice
    {
        public long amount { get; set; }
        public string currency { get; set; }
        public string currency_symbol { get; set; }
        public string price_string { get; set; }
        public PurchasePrice()
        {
            amount = 0;
            currency = "USD";
            currency_symbol = "$";
            price_string = "Free";
        }
    }
}
