/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */


$('a').click(function() {
    $('html, body').animate({
        scrollTop: $($.attr(this, 'href')).offset().top
    }, 1500);
    return false;
});