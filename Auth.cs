using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Globalization;

public class Auth
{

    private string refreshToken;
    private int expiresIn;
    public Auth() { }

    private void setRefreshToken(string token)
    {
        this.refreshToken = token;
    }

    private void setExpirationTimeInSeconds(int expTime)
    {
        this.expiresIn = expTime;
    }

    // public IEnumerator SignUpRequest(string email, string pass, System.Action<Dictionary<string, object>> callback)
    public IEnumerator SignUpRequest(string email, string pass, System.Action<Dictionary<string, object>> callback)
    {
        string body = "{ \"email\" : \"" + email + "\", \"password\" : \"" + pass + "\", \"returnSecureToken\": \"true\" }";
        UnityWebRequest www = UnityWebRequest.Post("https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=AIzaSyDCLTu_ulqmKzelXzCXLl5feEiFpOSyPgs",
            body, "application/json");
        Dictionary<string, object> response = new Dictionary<string, object>();
        // string response = "";
        yield return www.SendWebRequest();
        // se puede quedar trabado por falta de conexion 
        if (www.result != UnityWebRequest.Result.Success)
        {
            //Debug.Log(www.error);
            Dictionary<string, object> json = JsonHandler.ConvertStringJsonToDictionary(www.downloadHandler.text);
            object error;
            if (json.TryGetValue("error", out error))
            {

                Dictionary<string, object> errorDict = JsonHandler.ConvertStringJsonToDictionary(Convert.ToString(error));
                object message;
                if (errorDict.TryGetValue("message", out message))
                {
                   response.Add("result", false);
                   response.Add("message", FirebaseHumanResponses(Convert.ToString(message)));
                // response = Convert.ToString(message);
                }
            }
            else
            {
                Debug.Log("No se encontro el value error");
            }
        }
        else
        {
            response.Add("result", true);
            response.Add("message", "Se logueo con Exito");
            // response = "Se registro con Exito";
        }
        callback(response);
    }

    // public IEnumerator LoginRequest(string email, string password, System.Action<Dictionary<string, object>> callback)
    public IEnumerator LoginRequest(string email, string password, System.Action<Dictionary<string, object>> callback)
    {
        Dictionary<string, object> response = new Dictionary<string, object>();
        // string response = "";
        string body = "{ \"email\" : \"" + email + "\", \"password\" : \"" + password + "\", \"returnSecureToken\": \"true\" }";
        UnityWebRequest www = UnityWebRequest.Post("https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=AIzaSyDCLTu_ulqmKzelXzCXLl5feEiFpOSyPgs",
            body, "application/json");
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            //Debug.Log(www.error);
            Dictionary<string, object> json = JsonHandler.ConvertStringJsonToDictionary(www.downloadHandler.text);
            if (json != null && json.TryGetValue("error", out object error))
            {

                Dictionary<string, object> errorDict = JsonHandler.ConvertStringJsonToDictionary(Convert.ToString(error));
                object message;
                if (errorDict.TryGetValue("message", out message))
                {
                    response.Add("result", false);
                    response.Add("message", FirebaseHumanResponses(Convert.ToString(message)));
                    // response = Convert.ToString(message);

                }
            }
            else
            {
                response.Add("result", false);
                response.Add("message", "Ocurrio un error inesperado + \n" + www.error);
                // response = "Ocurrio un error inesperado + \n" + www.error;
            }
        }
        else
        {
            //JsonUtility.FromJson
            Dictionary<string, object> json = JsonHandler.ConvertStringJsonToDictionary(www.downloadHandler.text);

            response.Add("result", true);
            response.Add("message", "Se logueo con Exito");
            // response = "Se logueo con Exito";
            object idToken, refreshToken, expiresIn;
            if (json.TryGetValue("idToken", out idToken))
            {
                RealtimeDatabase.setIdToken(Convert.ToString(idToken));
            }
            if (json.TryGetValue("refreshToken", out refreshToken))
            {
                setRefreshToken(Convert.ToString(refreshToken));
            }
            if (json.TryGetValue("expiresIn", out expiresIn))
            {
                setExpirationTimeInSeconds(Convert.ToInt32(expiresIn));
            }
        }

        callback(response);

    }

    // Static Methods To Handle Some Responses from the Auth Service
    /*
        REFRESH TOKEN    

        TOKEN_EXPIRED
        USER_DISABLED
        USER_NOT_FOUND
        API key not valid
        INVALID_REFRESH_TOKEN
        Invalid JSON payload received
        INVALID_GRANT_TYPE
        MISSING_REFRESH_TOKEN
    */
    /*
        REGISTER
        
        EMAIL_EXISTS
        OPERATION_NOT_ALLOWED
        TOO_MANY_ATTEMPTS_TRY_LATER
        WEAK_PASSWORD
    */
    /*
        LOGIN
    
        EMAIL_NOT_FOUND
        INVALID_PASSWORD
        USER_DISABLED
    */

    private string FirebaseHumanResponses(string firebaseResponse)
    {
        //Debug.Log(firebaseResponse);
        if (string.IsNullOrEmpty(firebaseResponse))
        {
            return "Error en el servicio, por favor intente nuevamente en unos minutos";
        }
        if (firebaseResponse.Contains("EMAIL_NOT_FOUND")
            || firebaseResponse.Contains("INVALID_PASSWORD")
        )
        {
            return "Contraseña o Email Incorrectos, favor verificar";
        }
        if (firebaseResponse.Contains("INVALID_EMAIL"))
        {
            return "Favor colocar una direccion de correo valida";
        }
        if (firebaseResponse.Contains("EMAIL_EXISTS"))
        {
            return "Este email ya posee un usuario registrado";
        }
        if (firebaseResponse.Contains("WEAK_PASSWORD"))
        {
            return "La contraseña debe contener al menos 6 caracteres";
        }
        if (firebaseResponse.Contains("TOO_MANY_ATTEMPTS_TRY_LATER"))
        {
            return "Hubo demasiados intentos de acceso, por favor intente nuevamente mas tarde";
        }
        if (firebaseResponse.Contains("OPERATION_NOT_ALLOWED"))
        {
            return "Usted no esta autorizado a realizar esta accion";
        }

        return "Error desconocido";
    }
}
