/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

$(document).ready(function() {

    $('#show-additional-description').click(function() {
        document.getElementById('popup-description').style.display = 'block';
    });

    $('#hide-additional-description').click(function() {
        document.getElementById('popup-description').style.display = 'none';
    });


    $('.container-start h1').delay(2000).queue(function() {
        $(this).addClass('load-anim').dequeue();
    });

    $('.container-start h3').delay(3000).queue(function() {
        $(this).addClass('load-anim').dequeue();
    });

    $('.container-start p').delay(4000).queue(function() {
        $(this).addClass('load-anim').dequeue();
    });

    $('.decepticons-logo').delay(5000).queue(function() {
        $(this).addClass('load-anim').dequeue();
    });


    $('.decepticons-logo').delay(1000).queue(function() {
        $(this).addClass('load-rotate').dequeue();
    });

    $('.decepticons-logo').delay(1000).queue(function() {
        $(this).removeClass('load-rotate').dequeue();
    });
});

$(window).load(function() {
    $('.page-load-anim').fadeOut(2000);
});

$('a.nav-text').click(function() {
    $('html, body').animate({
        scrollTop: $($.attr(this, 'href')).offset().top
    }, 1500);
    return false;
});



var viewportHeight = $(window).height();
$(window).resize(function() {
    viewportHeight = $(window).height();
});


var targetHome = $('#home');


var targetDescription = $('#description.section-custom');
var descriptionFadeAnim = $('#description .read-fade');

var targetAbout = $('#about.section-custom');
var aboutFadeAnim = $('#about .read-fade');

var targetContact = $('#contact.section-custom');
var contactFadeAnim = $('#contact .read-fade');

$(window).scroll(function(event) {
    var scroll = $(window).scrollTop();
    applyFadeAnim(targetDescription, descriptionFadeAnim, scroll);
    applyFadeAnim(targetAbout, aboutFadeAnim, scroll);
    applyFadeAnim(targetContact, contactFadeAnim, scroll);

});

function applyFadeAnim(target, targetFadeAnim, scroll)
{
    if (scroll >= (target.offset().top - (viewportHeight / 2)) && scroll < (target.offset().top + (viewportHeight / 2)))
        targetFadeAnim.addClass('anim');
    else
        targetFadeAnim.removeClass('anim');
}

function focusNavigation(target, scroll)
{
    if (scroll >= target.offset().top)
        $('a[href="#' + target.attr('id').toString() + '"]');
}

setInterval(function() {
    $('.nav-download').addClass('download-blink').delay(500).queue(function() {
        $(this).removeClass('download-blink').dequeue();
    });
}, 1000);