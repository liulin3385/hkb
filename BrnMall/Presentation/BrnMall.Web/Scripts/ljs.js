<!--��Ʒ���� -->
$(document).ready(function(
	){
    $(".spfl").hover(
	function() {
		$(this).children(".menu").show()
	},
	function(){
		$(this).children(".menu").hide()
		}
	)	
});

<!--����չʾ -->
$(function()
   		{
	   $(".rzsj_yctup h3").toggle(function()
													 {
		 $("#lj_ycsj").hide(300)
			$(".rzsj_yctup h3").text("�鿴������Ϣ")
		 } ,
		function()
					{
		 $("#lj_ycsj").show(300)
		 $(".rzsj_yctup h3").text("����������Ϣ")
		 });
});


$(function()
   		{
	   $("#jr1").hover(function()
													 {
		 $("#conjr1").show()
		
		 } ,
		function()
					{
		 $("#conjr1").hide()
		
		 });
}); 
$(function()
   		{
	   $("#jr2").hover(function(){
		 $("#conjr2").show()
		
		 } ,
		function()
					{
		 $("#conjr2").hide()
		
		 });
});
$(function()
   		{
	   $(".flcanpn ul li").hover(function(){
		 $(this).find(".l_hovte").show()
		
		 } ,
		function()
					{
		 $(this).find(".l_hovte").hide()
		
		 });
});