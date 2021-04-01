using System.Collections.Generic;

namespace UdemyGrabberWPF.Models
{
    public class CheckoutSubmitRequest
    {
        public string checkout_environment { get; set; }
        public string checkout_event { get; set; }
        public PaymentInfo payment_info { get; set; }
        public ShoppingInfo shopping_info { get; set; }
        public CheckoutSubmitRequest()
        {
            checkout_environment = "Marketplace";
            checkout_event = "Submit";
            payment_info = new PaymentInfo();
            shopping_info = new ShoppingInfo();
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
    public class ShoppingInfo
    {
        public bool is_cart { get; set; }
        public List<Item> items { get; set; }
        public ShoppingInfo()
        {
            is_cart = false;
            items = new List<Item>();
            items.Add(new Item());
        }
    }
    public class Item
    {
        public Buyable buyable { get; set; }
        public long buyableId { get; set; }
        public string buyableType { get; set; }
        public DiscountInfo discountInfo { get; set; }
        public Price price { get; set; }
        public Item()
        {
            buyable = new Buyable();
            buyableId = 0;
            buyableType = "course";
            discountInfo = new DiscountInfo();
            price = new Price();
        }
    }
    public class Buyable
    {
        public Context context { get; set; }
        public long id { get; set; }
        public string type { get; set; }
        public Buyable()
        {
            context = new Context();
            type = "course";
        }
    }
    public class Context { }
    public class DiscountInfo
    {
        public string code { get; set; }
    }
    public class Price
    {
        public long amount { get; set; }
        public string currency { get; set; }
        public Price()
        {
            amount = 0;
            currency = "USD";
        }
    }
}
