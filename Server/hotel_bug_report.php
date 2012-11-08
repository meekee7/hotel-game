<?php
   if (!empty($_GET['text']))
      $texto = $_GET['text'];
   else
      $texto = "";

   if (!empty($_GET['lang']))
      $lang = $_GET['lang'];
   else
      $lang = "es";

   if ($texto == "")
   {
      if ($lang == "es-ES" || $lang == "es_ES" || $lang="es")
         echo "'text' no especificado";
      else
         echo "'text' not specified";
   }
   else
   {
      $email = "ifilgud@gmail.com";
      $from = "From: Bugs_hotel@hotel-game.com";
      $subject = "Reporte de bug de Hotel";
      if (mail($email, $subject, $texto, $from))
      {
         if ($lang == "es-ES" || $lang == "es_ES" || $lang="es")
            echo "Correo enviado correctamente :)";
         else
            echo "Email sent successfully :)";
      }
      else
      {
         if ($lang == "es-ES" || $lang == "es_ES" || $lang="es")
            echo "Error enviando correo a ifilgud@gmail.com :( (Escribe un correo manualmente avisando del problema por favor)";
         else
            echo "Error sending an email to ifilgud@gmail.com :( (Please write an email manually to warn about the problem!)";
      }
   } 
?>
