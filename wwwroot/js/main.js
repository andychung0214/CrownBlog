$(function() {
  "use strict";

  var nav_offset_top = $('header').height() + 50; 
    /*-------------------------------------------------------------------------------
	  Navbar 
	-------------------------------------------------------------------------------*/

	//* Navbar Fixed  
    function navbarFixed(){
        if ( $('.header_area').length ){ 
            $(window).scroll(function() {
                var scroll = $(window).scrollTop();   
                if (scroll >= nav_offset_top ) {
                    $(".header_area").addClass("navbar_fixed");
                } else {
                    $(".header_area").removeClass("navbar_fixed");
                }
            });
        };
    };
    navbarFixed();


  if ($('.blog-slider').length) {
    $('.blog-slider').owlCarousel({
        loop: true,
        margin: 30,
        items: 1,
        nav: true,
        autoplay: 2500,
        smartSpeed: 1500,
        dots: false,
        responsiveClass: true,
        navText: ["<div class='blog-slider__leftArrow'><img src='https://andychung0214.synology.me/images/css-img/left-arrow.png'></div>","<div class='blog-slider__rightArrow'><img src='https://andychung0214.synology.me/images/css-img/right-arrow.png'></div>"],
        responsive:{
          0:{
              items:1
          },
          600:{
              items:2
          },
          1000:{
              items:3
          }
      }
    })
  }



  //------- mailchimp --------//  
	function mailChimp() {
		$('#mc_embed_signup').find('form').ajaxChimp();
    }

    function initForm() {
        $('#formTitle').val("");
        $('#formDesc').val("");
        //$('#Msg').val("");
        //$('#CaptchaCode').val("");
    }

    function getFormInfo() {
        $('formTitle');
    }

    //$("#formSubmit").on('click', function () {
    //    var formTitle = $('#formTitle').val();
    //    var formDesc = $('#formDesc').val();
    //    //var mainModel = {
    //    //    Title: formTitle,
    //    //    Description: formDesc
    //    //};

    //    //var api_url = '/api/v1/blog/articles/add';
    //    var api_url = '/api/cms/CreateArticle';
    //    $.ajax({
    //        url: api_url,
    //        type: 'POST',
    //        contentType: 'application/json',
    //        data: JSON.stringify({
    //            mainModel:{
    //                Title: formTitle,
    //                Description: formDesc
    //            }
    //        })
    //    }).done(function (data) {
    //        if (data.IsSuccess) {
    //            alert('Add Article Success!');
    //            //initForm();
    //        } else {
    //            alert('Send Mail Failed. Please check all required info');
    //        }
    //        }).fail(function (jqXHR, textStatus, errorThrown) {
    //            alert(jqXHR);
    //            alert(jqXHR.responseText);
    //    });
    //});


    mailChimp();
    //initForm();
    //getFormInfo();
    //submitForm();


    // 6 create an instance when the DOM is ready
    $('#jstree').jstree();
    // 7 bind to events triggered on the tree
    $('#jstree').on("changed.jstree", function (e, data) {
        console.log(data.selected);
        var href = data.node.a_attr.href;
        document.location.href = href;
    });

    // 8 interact with the tree - either way is OK
    //$('#treeDemobutton').on('click', function () {
    //    $('#jstree').jstree(true).select_node('child_node_1');
    //    $('#jstree').jstree('select_node', 'child_node_1');
    //    $.jstree.reference('#jstree').select_node('child_node_1');
    //});


    $("#datepicker").datepicker({
        dateFormat: "yy-mm-dd" //修改顯示順序
    });

    $(".timepicker").timepicker({
        timeFormat: "h:mm p", // 時間隔式
        interval: 30, //時間間隔
        minTime: "06", //最小時間
        maxTime: "23:55pm", //最大時間
        defaultTime: "06", //預設起始時間
        startTime: "01:00", // 開始時間
        dynamic: true, //是否顯示項目，使第一個項目按時間順序緊接在所選時間之後
        dropdown: true, //是否顯示時間條目的下拉列表
        scrollbar: false //是否顯示捲軸
    });

});


