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
        <link rel="shortcut icon" href="img/DecepticonLogo.png">
    </head>
    <body>

        <div class="page-load-anim">
            <img class="decepticons-logo-progress-white" src="img/DecepticonLogo.png" />
            <p>Loading...</p>
        </div>

        <nav class="navbar custom-nav navbar-fixed-top">
            <div class="container">

                <div class="navbar-header">
                    <button type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#navbar-collapse-target">
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                    </button>


                </div>

                <!--                <div class="decepticons-logo-nav">
                                        <img class="decepticons-logo-nav-img" src="img/DecepticonLogo.png" />
                                </div>-->

                <div class="collapse navbar-collapse" id="navbar-collapse-target">


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
            </div>
        </nav>


        <div class="read-fade-home">
        </div>
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
            <div class="read-fade">
            </div>

            <div id="popup-description" class="popup-content">

                <a id="hide-additional-description" href = "javascript:void(0)">X</a>

                <h4>Server</h4>

                <p> The game map is created by the server which contains brick, stone, water and empty blocks.
                    Client's can join to the server by sending the JOIN# message and server will reply a message which 
                    contains either information or error.Server can support up to maximum 5 client's.After the game start 
                    server broadcast the game update message's to the client's for each 1 second.
                </p>
                <br>
                <br>
                <h4>Game Rules</h4>

                <p> 
                    &bull; Tanks can only move to north east south and west directions.<br>
                    &bull; Tank shell's are move 4 times faster than tanks.<br>
                    &bull; Tanks can shoot over water.but not through the stones and bricks.<br>
                    &bull; Bricks can take 25% damage if a tank shell hit.<br>
                    &bull; Hitting a tank shell may lose 10% of health.<br>
                    &bull; If a tank tries to move on the water it get destroyed.<br>
                    &bull; Coins and life packs are randomly appear in the map.<br>
                    &bull; A life pack can heal a tank by 20%.<br>
                    &bull; Collecting coins, destroying other tanks, destroying bricks will help to increase the score.<br>
                    &bull; Game will end when all client's are dead or game time is expired or all coin piles are finished.<br>               
                </p>
                <br>
                <br>
                <h4>Client</h4>

                <p>
                    To build the client we use Unity game engine for rendering 3D game view and C# for the game logic.<br>
                    We are expect to use Minimax and Alpha-beta pruning algorithm's for the AI.
 
                </p>

            </div>


            <div class="section-content">
                <div class="row">
                    <div class="col-md-12">
                        <h2>Description</h2>
                        <p>This game is a server client based tank game and server support up to maximum 5 clients.<br>
                            The goal of this game is to defeat the other tanks while you collecting the maximum number of coins.
                        </p>

                        <a id="show-additional-description" href = "javascript:void(0)">Show more</a>


                    </div>
                </div>
            </div>
        </section>
        <hr>

        <section id="about" class="container-fluid section-custom text-center">
            <div class="read-fade">
            </div>
            <div class="section-content">
                <div class="row">
                    <div class="col-md-12">
                        <h2>About</h2>
                        <p>We are semester 4 computer science and engineering students in University Of Moratuwa</p> 

                        <p> This is the the implementation of our website for the programming chalenge 2 project.</p>

                        <p>In this project we are developing a game client which will connect to a server and play a
                            tank game with other client applications. the server provides the basic fundamentals for 
                            the game and it controls the environment. our client can send instruction to the server 
                            to control our avatar (Tank) in the game. and the Server updates all the clients about the
                            information about other avatar's (Tank) periodically.</p>

                        <p>
                            The objective is to develop the best game client with better algorithms for understand 
                            and defeat other game clients while maximizing the score.<br>
                        </p>
                    </div>
                </div>
            </div>
        </section>

        <hr>

        <section id="contact" class="container-fluid section-custom text-center">
            <div class="read-fade">
            </div>
            <div class="section-content">
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
            </div>
        </section>

        <hr>

        <footer>
            <div class="container-fluid text-center footer-custom">

                <p>Copyright &copy;  2015</p>

            </div>
        </footer>

        <script src="js/jquery.js"></script>
        <script src="js/bootstrap.min.js"></script>
        <script src="js/custom.js"></script>
		<script>
		  (function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){
		  (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),
		  m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
		  })(window,document,'script','//www.google-analytics.com/analytics.js','ga');

		  ga('create', 'UA-60153562-1', 'auto');
		  ga('send', 'pageview');

		</script>
    </body>
</html>
