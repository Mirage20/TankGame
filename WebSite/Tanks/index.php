<!DOCTYPE html>
<!--
To change this license header, choose License Headers in Project Properties.
To change this template file, choose Tools | Templates
and open the template in the editor.
-->
<html>
    <head>
        <meta charset="utf-8"> 
        <meta http-equiv="X-UA-Compatible" content="IE=edge">
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <title>Tanks</title>

        <link href="css/bootstrap.min.css" rel="stylesheet">
        <link href="css/custom.css" rel="stylesheet">
    </head>
    <body>

        <nav class="navbar custom-nav navbar-fixed-top">
            <div class="container">           
                <div class="navbar-left">
                    <ul class="nav navbar-nav">
                        <li >
                            <a class="nav-text nav-home" href="#home">Home</a>
                        </li>
                        <li >
                            <a class="nav-text" href="#description">Description</a>
                        </li>
                        <li >
                            <a class="nav-text" href="#about">About</a>
                        </li>              
                        <li>
                            <a class="nav-text" href="#contact">Contact us</a>
                        </li>
                    </ul>
                </div>   


                <div class="navbar-right">
                    <ul class="nav navbar-nav">
                        <li >
                            <a class="nav-text nav-download" href="#download">Download</a>
                        </li>
                    </ul>
                </div>  
            </div>
        </nav>


        <header id="home" class="container-start">
            <div class="container">
                <div class="row">
                    <div class="col-md-12 ">
                        <h1>Tanks Game Client</h1>
                        <h3>Are you ready to play with decepticons?</h3>
                        <br>
                        <p>Play against the server.<br>Beat other opponents using a powerful AI build with the client.</p>                        
                        <img class="decepticons-logo" src="img/DecepticonLogo.png" />

                    </div>
                </div>
            </div>
        </header>

        <hr>

        <section id="description" class="container-fluid section-custom text-center">
            <div class="row">
                <div class="col-md-12">
                    <h2>Description</h2>
                    <p>Coming soon</p>
                </div>
            </div>
        </section>
        <hr>
        <section id="about" class="container-fluid section-custom text-center">
            <div class="row">
                <div class="col-md-12">
                    <h2>About</h2>
                    <p>We are semester 4 computer science and engineering students in University Of Moratuwa</p> 

                    <p> This is the the implementation of our website for the programming chalenge 2 project.</p>

                    <p>In this project we are developing a game client which will connect to a server and play a
                        tank game with other client applications. the server provides the basic fundamentals for 
                        the game and it controls the environment. our client can send instruction to the server 
                        to control our avatar (TANK) in the game. and the Server updates all the clients about the
                        information about other avatars (TANKS) periodically.</p>

                    <p>The objective is to develop the best game client with better algorithms for understand and defeat other game clients.<br>

                        Every game client has an graphical user interface and it indicates the status if the game. 
                        we have used unity game engine to develop the user interface of the game client. 
                        And we used c# to write the game logic.
                    </p>
                </div>
            </div>
        </section>

        <hr>

        <section id="contact" class="container-fluid section-custom text-center">
            <div class="row">
                <div class="col-md-12">
                    <h2>Contact us</h2>
                    <div class="social-contact">

                        <p>Adiesha Liyanage - Computer Science And Engineering Student</p> </p>
                        <ul class="list-inline">
                            <li>
                                <a href="https://www.facebook.com/adiesha.liyanage" class="social-facebook" target="_blank"></a>
                            </li>
                            <li>
                                <a href="https://twitter.com/Adiesha_" class="social-twitter" target="_blank"></a>
                            </li>
                            <li>
                                <a href="https://plus.google.com/u/0/103418527625565866499" class="social-googleplus" target="_blank"></a>
                            </li>
                        </ul>

                        <p>Mirage Abeysekara - Computer Science And Engineering Student</p> </p>
                        <ul class="list-inline">
                            <li>
                                <a href="https://www.facebook.com/MiRAGECreator" class="social-facebook" target="_blank"></a>
                            </li>
                            <li>
                                <a href="https://twitter.com/MiRAGECreator" class="social-twitter" target="_blank"></a>
                            </li>
                            <li>
                                <a href="https://plus.google.com/u/0/+MirageAbeysekara" class="social-googleplus" target="_blank"></a>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
        </section>

        <hr>
        
        <footer>
            <div class="container-fluid text-center">

                <p>Copyright &copy;  2015</p>

            </div>
        </footer>
        
        <script src="js/jquery.js"></script>
        <script src="js/custom.js"></script>
    </body>
</html>
