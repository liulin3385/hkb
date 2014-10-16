using System;

namespace BrnMall.Core
{
    /// <summary>
    /// 订单商品信息类
    /// </summary>
    public class OrderProductInfo
    {
        private int _recordid;//记录id
        private int _oid = 0;//订单id
        private int _uid = 0;//用户id
        private string _sid = "";//sessionId
        private int _pid;//商品id
        private string _psn;//商品编码
        private int _cateid;//分类id
        private int _brandid;//品牌id
        private int _storeid;//店铺id
        private int _storecid;//店铺分类id
        private int _storestid = 0;//店铺配送模板id
        private string _name;//商品名称
        private string _showimg;//商品展示图片
        private decimal _discountprice;//商品折扣价格
        private decimal _shopprice;//商品商城价格
        private decimal _costprice;//商品成本价格
        private decimal _marketprice;//商品市场价格
        private int _weight;//商品重量
        private int _isreview;//是否评价(0代表未评价，1代表已评价)
        private int _realcount = 0;//真实数量
        private int _buycount = 0;//商品购买数量
        private int _sendcount = 0;//商品邮寄数量
        private int _type = 0;//商品类型(0为普遍商品，1为赠品，2为套装商品，3为满赠商品)
        private int _paycredits = 0;//支付积分
        private int _coupontypeid = 0;//赠送优惠劵类型id
        private int _extcode1 = 0;//扩展1
        private int _extcode2 = 0;//扩展2
        private int _extcode3 = 0;//扩展3
        private int _extcode4 = 0;//扩展4
        private int _extcode5 = 0;//扩展5
        private int _extcode6 = 0;//扩展6
        private int _extcode7 = 0;//扩展7
        private int _extcode8 = 0;//扩展8
        private int _extcode9 = 0;//扩展9
        private int _extcode10 = 0;//扩展10
        private int _extcode11 = 0;//扩展11
        private int _extcode12 = 0;//扩展12
        private string _extstr1 = "";//扩展字符串1
        private string _extstr2 = "";//扩展字符串2
        private string _extstr3 = "";//扩展字符串3
        private string _extstr4 = "";//扩展字符串4
        private string _ip;//ip地址
        private DateTime _addtime;//添加时间

        /// <summary>
        /// 记录id
        /// </summary>
        public int RecordId
        {
            get { return _recordid; }
            set { _recordid = value; }
        }
        /// <summary>
        /// 订单id
        /// </summary>
        public int Oid
        {
            get { return _oid; }
            set { _oid = value; }
        }
        /// <summary>
        /// 用户id
        /// </summary>
        public int Uid
        {
            get { return _uid; }
            set { _uid = value; }
        }
        /// <summary>
        /// sessionId
        /// </summary>
        public string Sid
        {
            get { return _sid; }
            set { _sid = value.TrimEnd(); }
        }
        /// <summary>
        /// 商品id
        /// </summary>
        public int Pid
        {
            get { return _pid; }
            set { _pid = value; }
        }
        /// <summary>
        /// 商品编码
        /// </summary>
        public string PSN
        {
            get { return _psn; }
            set { _psn = value.TrimEnd(); }
        }
        /// <summary>
        /// 分类id
        /// </summary>
        public int CateId
        {
            get { return _cateid; }
            set { _cateid = value; }
        }
        /// <summary>
        /// 品牌id
        /// </summary>
        public int BrandId
        {
            get { return _brandid; }
            set { _brandid = value; }
        }
        /// <summary>
        /// 店铺id
        /// </summary>
        public int StoreId
        {
            get { return _storeid; }
            set { _storeid = value; }
        }
        /// <summary>
        /// 店铺分类id
        /// </summary>
        public int StoreCid
        {
            get { return _storecid; }
            set { _storecid = value; }
        }
        /// <summary>
        /// 店铺配送模板id
        /// </summary>
        public int StoreSTid
        {
            set { _storestid = value; }
            get { return _storestid; }
        }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// 商品展示图片
        /// </summary>
        public string ShowImg
        {
            get { return _showimg; }
            set { _showimg = value; }
        }
        /// <summary>
        /// 商品折扣价格
        /// </summary>
        public decimal DiscountPrice
        {
            get { return _discountprice; }
            set { _discountprice = value; }
        }
        /// <summary>
        /// 商品商城价格
        /// </summary>
        public decimal ShopPrice
        {
            get { return _shopprice; }
            set { _shopprice = value; }
        }
        /// <summary>
        /// 商品成本价格
        /// </summary>
        public decimal CostPrice
        {
            get { return _costprice; }
            set { _costprice = value; }
        }
        /// <summary>
        /// 商品市场价格
        /// </summary>
        public decimal MarketPrice
        {
            get { return _marketprice; }
            set { _marketprice = value; }
        }
        /// <summary>
        /// 商品重量
        /// </summary>
        public int Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }
        /// <summary>
        /// 是否评价(0代表未评价，1代表已评价)
        /// </summary>
        public int IsReview
        {
            get { return _isreview; }
            set { _isreview = value; }
        }
        /// <summary>
        /// 真实数量
        /// </summary>
        public int RealCount
        {
            get { return _realcount; }
            set { _realcount = value; }
        }
        /// <summary>
        /// 商品购买数量
        /// </summary>
        public int BuyCount
        {
            get { return _buycount; }
            set { _buycount = value; }
        }
        /// <summary>
        /// 商品邮寄数量
        /// </summary>
        public int SendCount
        {
            get { return _sendcount; }
            set { _sendcount = value; }
        }
        /// <summary>
        /// 商品类型(0为普遍商品，1为赠品，2为套装商品，3为满赠商品)
        /// </summary>
        public int Type
        {
            get { return _type; }
            set { _type = value; }
        }
        /// <summary>
        /// 支付积分
        /// </summary>
        public int PayCredits
        {
            get { return _paycredits; }
            set { _paycredits = value; }
        }
        /// <summary>
        /// 赠送优惠劵类型id
        /// </summary>
        public int CouponTypeId
        {
            get { return _coupontypeid; }
            set { _coupontypeid = value; }
        }
        /// <summary>
        /// 扩展1
        /// </summary>
        public int ExtCode1
        {
            get { return _extcode1; }
            set { _extcode1 = value; }
        }
        /// <summary>
        /// 扩展2
        /// </summary>
        public int ExtCode2
        {
            get { return _extcode2; }
            set { _extcode2 = value; }
        }
        /// <summary>
        /// 扩展3
        /// </summary>
        public int ExtCode3
        {
            get { return _extcode3; }
            set { _extcode3 = value; }
        }
        /// <summary>
        /// 扩展4
        /// </summary>
        public int ExtCode4
        {
            get { return _extcode4; }
            set { _extcode4 = value; }
        }
        /// <summary>
        /// 扩展5
        /// </summary>
        public int ExtCode5
        {
            get { return _extcode5; }
            set { _extcode5 = value; }
        }
        /// <summary>
        /// 扩展6
        /// </summary>
        public int ExtCode6
        {
            get { return _extcode6; }
            set { _extcode6 = value; }
        }
        /// <summary>
        /// 扩展7
        /// </summary>
        public int ExtCode7
        {
            get { return _extcode7; }
            set { _extcode7 = value; }
        }
        /// <summary>
        /// 扩展8
        /// </summary>
        public int ExtCode8
        {
            get { return _extcode8; }
            set { _extcode8 = value; }
        }
        /// <summary>
        /// 扩展9
        /// </summary>
        public int ExtCode9
        {
            get { return _extcode9; }
            set { _extcode9 = value; }
        }
        /// <summary>
        /// 扩展10
        /// </summary>
        public int ExtCode10
        {
            get { return _extcode10; }
            set { _extcode10 = value; }
        }
        /// <summary>
        /// 扩展11
        /// </summary>
        public int ExtCode11
        {
            get { return _extcode11; }
            set { _extcode11 = value; }
        }
        /// <summary>
        /// 扩展12
        /// </summary>
        public int ExtCode12
        {
            get { return _extcode12; }
            set { _extcode12 = value; }
        }
        /// <summary>
        /// 扩展字符串1
        /// </summary>
        public string ExtStr1
        {
            get { return _extstr1; }
            set { _extstr1 = value; }
        }
        /// <summary>
        /// 扩展字符串2
        /// </summary>
        public string ExtStr2
        {
            get { return _extstr2; }
            set { _extstr2 = value; }
        }
        /// <summary>
        /// 扩展字符串3
        /// </summary>
        public string ExtStr3
        {
            get { return _extstr3; }
            set { _extstr3 = value; }
        }
        /// <summary>
        /// 扩展字符串4
        /// </summary>
        public string ExtStr4
        {
            get { return _extstr4; }
            set { _extstr4 = value; }
        }
        /// <summary>
        /// ip地址
        /// </summary>
        public string IP
        {
            get { return _ip; }
            set { _ip = value; }
        }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime
        {
            get { return _addtime; }
            set { _addtime = value; }
        }
    }
}

