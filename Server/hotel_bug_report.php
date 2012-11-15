<?php
   if (!empty($_GET['text']))
      $texto = $_GET['text'];
   else
      $texto = "";

   if (!empty($_GET['lang']))
      $lang = $_GET['lang'];
   else
      $lang = "es";

   if (!empty($_GET['email']))
      $email_orig = $_GET['email'];
   else
      $email_orig = "Email no especificado";

   if (($email_orig == "") || ($email_orig == "Email no especificado"))
   {
      if ($lang == "es")
         echo "'email' no especificado";
      else
         echo "'email' not specified";
      return;
   }

   if ($texto == "")
   {
      if ($lang == "es")
         echo "'text' no especificado";
      else
         echo "'text' not specified";
   }
   else
   {
      $email = "ifilgud@gmail.com";
      $from = "From: Bugs_hotel@hotel-game.com";
      $subject = "Reporte de bug de Hotel";
      $texto .= "\n\nEmail: " . $email_orig;
      if (mail($email, $subject, $texto, $from))
      {
         if ($lang == "es")
            echo "Correo enviado correctamente :) ¡Gracias!";
         else
            echo "Email sent successfully :) Thanks!";
      }
      else
      {
         if ($lang == "es")
            echo "Error enviando correo a ifilgud@gmail.com :( (Escribe un correo manualmente avisando del problema por favor)";
         else
            echo "Error sending an email to ifilgud@gmail.com :( (Please write an email manually to warn about the problem!)";
      }
   } 
?>
