using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class RealtimeDatabase
{

    private static string idToken;
    public RealtimeDatabase() { }

    public static void setIdToken(string token)
    {
        idToken = token;
    }

    /*
    private static RealtimeDatabase instance;
    public static RealtimeDatabase Instance
    {
        get
        {
            if (instance == null) instance = new RealtimeDatabase();
            return instance;
        }
    }
    */

    /**
     * Metodo para retornar todos los datos de la coleccion
     * 
     *  string collection
     *      Path de la coleccion en la que se quiere trabajar
     *      por ejemplo usuarios o una subestructura nivel1/stage3/parametros
     *  System.Action<Dictionary<string, object>> callback
     *      Funcion de recuperacion de datos en el Coroutine de la llamada
     * 
     */
    public IEnumerator GetAllData(string collection, System.Action<Dictionary<string, object>> callback)
    {
        return GetData(collection, null, 0, 0, null, null, null, callback);
     
    }

    /**
     * Metodo para retornar todos los datos de la coleccion que correspondan a una key 
     * 
     *  string collection
     *      Path de la coleccion en la que se quiere trabajar
     *      por ejemplo usuarios o una subestructura nivel1/stage3/parametros
     *  string key
     *      Llave que se quiere buscar en la peticion, puede ser el nombre de un campo
     *      o los comodines $key o $value para buscar por el key de un objeto o el value
     *      de un objeto
     *  string value
     *      Valor que sera comparado con la igualdad, se buscara exactamente este valor
     *  System.Action<Dictionary<string, object>> callback
     *      Funcion de recuperacion de datos en el Coroutine de la llamada
     * 
     */
    public IEnumerator GetDataBy(string collection, string key, string value, System.Action<Dictionary<string, object>> callback)
    {
        return GetData(collection, key, 0, 0, null, null, value, callback);
    }


    /**
     * Metodo para retornar todos los datos de la coleccion que tengan en comun la key del objeto
     * 
     *  string collection
     *      Path de la coleccion en la que se quiere trabajar
     *      por ejemplo usuarios o una subestructura nivel1/stage3/parametros
     *  string value
     *      Valor que sera comparado con la igualdad, se buscara exactamente este valor
     *  System.Action<Dictionary<string, object>> callback
     *      Funcion de recuperacion de datos en el Coroutine de la llamada
     * 
     */
    public IEnumerator GetDataByKey(string collection, string value, System.Action<Dictionary<string, object>> callback)
    {
        return GetData(collection, "$key", 0, 0, null, null, value, callback);
    }



    /**
     * Metodo para retornar todos los datos de la coleccion que tengan en comun algun value del objeto
     * 
     *  string collection
     *      Path de la coleccion en la que se quiere trabajar
     *      por ejemplo usuarios o una subestructura nivel1/stage3/parametros
     *  string value
     *      Valor que sera comparado con la igualdad, se buscara exactamente este valor
     *  System.Action<Dictionary<string, object>> callback
     *      Funcion de recuperacion de datos en el Coroutine de la llamada
     * 
     */
    public IEnumerator GetDataByValue(string collection, string value, System.Action<Dictionary<string, object>> callback)
    {
        return GetData(collection, "$value", 0, 0, null, null, value, callback);
    }


    /**
    * Metodo para retornar todos los objetos de la coleccion desde el primer objeto cargado 
    * hasta el numero indicado
    * 
    *  string collection
    *      Path de la coleccion en la que se quiere trabajar
    *      por ejemplo usuarios o una subestructura nivel1/stage3/parametros
   *  int limit
   *      numero maximo de registros para ver desde el ultimo
    *  System.Action<Dictionary<string, object>> callback
    *      Funcion de recuperacion de datos en el Coroutine de la llamada
    * 
    */
    public IEnumerator GetDataLimitToFirst(string collection, int limit, System.Action<Dictionary<string, object>> callback)
    {
        return GetData(collection, "$key", limit, 0, null, null, null, callback);
    }


    /**
   * Metodo para retornar todos los objetos de la coleccion desde el ultimo objeto cargado 
   * hasta el numero indicado hacia atras
   * 
   *  string collection
   *      Path de la coleccion en la que se quiere trabajar
   *      por ejemplo usuarios o una subestructura nivel1/stage3/parametros
   *  int limit
   *      numero maximo de registros para ver desde el primero
   *  System.Action<Dictionary<string, object>> callback
   *      Funcion de recuperacion de datos en el Coroutine de la llamada
   * 
   */
    public IEnumerator GetDataLimitToLast(string collection, int limit, System.Action<Dictionary<string, object>> callback)
    {
        return GetData(collection, "$key", 0, limit, null, null, null, callback);
    }



    /**
     * Metodo Generico de consulta de Datos
     * 
     *  string collection
     *      Path de la coleccion en la que se quiere trabajar
     *      por ejemplo usuarios o una subestructura nivel1/stage3/parametros
     *  string orderByKey
     *      Es el elemento sobre el cual se estaran realizando las distintas 
     *      operaciones de filtrado
     *  int limitToFirst
     *      recibe un integer que le indicara la cantidad de registros que
     *      se quieren recuperar desde el primer resultado
     *  int limitToLast
     *      recibe un integer que le indicara la cantidad de registros que
     *      se quieren recuperar desde el ultimo resultado
     *  string startAt
     *      <unknown behaviour>
     *  string endAt
     *      <unknown behaviour>
     *  string equalTo
     *      recibe un string, pero se puede utilizar para comparar cualquier cosa
     *      realmente utilizando la igualdad absoluta.
     *  System.Action<Dictionary<string, object>> callback
     *      Funcion de recuperacion de datos en el Coroutine de la llamada
     * 
     */
    public IEnumerator GetData(
        string collection, 
        string orderByKey, 
        int limitToFirst, 
        int limitToLast, 
        string startAt, 
        string endAt, 
        string equalTo, 
        System.Action<Dictionary<string, object>> callback
        )
    {
        Dictionary<string, object> response = new Dictionary<string, object>();
        // TODO: Agregar seguridad por token en el futuro al final de la url con "?auth="+ token_id
        string url = "https://odin-game-7edee-default-rtdb.firebaseio.com/" + collection + ".json";
        if ( !string.IsNullOrEmpty(orderByKey) )
        {
            url += "?orderBy=\"" + orderByKey + "\"";
            if (!string.IsNullOrEmpty(startAt))
            {
                url += "&startAt=\"" + startAt + "\"";
            }
            if (!string.IsNullOrEmpty(endAt))
            {
                url += "&endAt=\"" + endAt + "\"";
            }
            if (!string.IsNullOrEmpty(equalTo))
            {
                url += "&equalTo=\"" + equalTo + "\"";
            }
            if (limitToFirst > 0)
            {
                url += "&limitToFirst=" + Convert.ToString(limitToFirst);
            }
            if (limitToLast > 0)
            {
                url += "&limitToLast=" + Convert.ToString(limitToLast);
            }
        }
        //
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        //Debug.Log(www.result);
        // TODO: se puede quedar trabado por falta de conexion 
        if (www.result != UnityWebRequest.Result.Success || www.downloadHandler.text == "{}")
        {
            response.Add("result", false);
            response.Add("message", Convert.ToString(www.downloadHandler.text));
            response.Add("error", Convert.ToString(www.error));
        }
        else
        {
            //JsonUtility.FromJson
            response.Add("result", true);
            response.Add("data", www.downloadHandler.text) ;

        }
        callback(response);

    }






    /**
     *  Dictionary<string, object> data 
     *      Es el objeto que contendra el json a ser enviado en el servicio web
     *  string collection
     *      Path de la coleccion en la que se quiere agregar datos
     *      por ejemplo usuarios o una subestructura nivel1/stage3/parametros
     *  System.Action<Dictionary<string, object>> callback
     *      Funcion de recuperacion de datos en el Coroutine de la llamada
     * 
     */
    public IEnumerator SaveData(Dictionary<string, object> data, string collection, System.Action<Dictionary<string, object>> callback)
    {
        string body = JsonHandler.ConvertDictionaryToJson(data);
        Dictionary<string, object> response = new Dictionary<string, object>();
        // TODO: Agregar seguridad por token en el futuro al final de la url con "?auth="+ token_id
        string url = "https://odin-game-7edee-default-rtdb.firebaseio.com/" + collection + ".json";
        //
        UnityWebRequest www = UnityWebRequest.Post(url, body, "application/json");
        yield return www.SendWebRequest();
        //Debug.Log(www.result);
        string result = www.downloadHandler.text;
        // TODO: se puede quedar trabado por falta de conexion 
        if (www.result != UnityWebRequest.Result.Success)
        {
            //Debug.Log(www.error);
            response.Add("result", false);
            response.Add("message", result);
        }
        else
        {
            //JsonUtility.FromJson
            response.Add("result", true);
            response.Add("data", result);

        }
        callback(response);

    }

    /**
     *  Dictionary<string, object> data 
     *      Es el objeto que contendra el json a ser enviado en el servicio web
     *  string collection
     *      Path de la coleccion en la que se quiere agregar datos
     *      por ejemplo usuarios o una subestructura nivel1/stage3/parametros
     *  string id
     *      Identificador autogenerado por firebase, puede ser solo eso o un path 
     *      especifico para modificar 
     *      por ejemplo usuario/id/.json
     *      usuario/id/permisos.json
     *  System.Action<Dictionary<string, object>> callback
     *      Funcion de recuperacion de datos en el Coroutine de la llamada
     * 
     */
    // public IEnumerator UpdateData(Dictionary<string, object> data, string collection, string id, System.Action<Dictionary<string, object>> callback)
    public IEnumerator UpdateData(Dictionary<string, object> data, string collection, System.Action<Dictionary<string, object>> callback)
    {
        string body = JsonHandler.ConvertDictionaryToJson(data);
        Dictionary<string, object> response = new Dictionary<string, object>();
        // TODO: Agregar seguridad por token en el futuro al final de la url con "?auth="+ token_id
        // string url = "https://odin-game-7edee-default-rtdb.firebaseio.com/" + collection + "/" + id + ".json";
        string url = "https://odin-game-7edee-default-rtdb.firebaseio.com/" + collection + ".json";

        /* You may need to add header(s) */
        // request.SetRequestHeader("Content-Type", "application/json");
        UnityWebRequest www = UnityWebRequest.Post(url, body, "application/json");
        www.method = "PATCH";
        yield return www.SendWebRequest();
        //Debug.Log(www.result);
        // TODO: se puede quedar trabado por falta de conexion 
        if (www.result != UnityWebRequest.Result.Success)
        {
            //Debug.Log(www.error);
            response.Add("result", false);
            response.Add("message", www.downloadHandler.text);
        }
        else
        {
            //JsonUtility.FromJson
            response.Add("result", true);
            response.Add("data", www.downloadHandler.text);

        }
        callback(response);

    }



    /**
     *  
     *  string collection
     *      Path de la coleccion en la que se quiere agregar datos
     *      por ejemplo usuarios o una subestructura nivel1/stage3/parametros
     *  string id
     *      Identificador autogenerado por firebase, puede ser solo eso o un path 
     *      especifico para eliminar 
     *      por ejemplo usuario/id/.json
     *      usuario/id/permisos.json
     *  System.Action<Dictionary<string, object>> callback
     *      Funcion de recuperacion de datos en el Coroutine de la llamada
     * 
     */
    public IEnumerator DeleteData(string collection, string id, System.Action<Dictionary<string, object>> callback)
    {
        // string body = ConvertDictionaryToJson(data);
        // TODO: Agregar seguridad por token en el futuro al final de la url con "?auth="+ token_id
        Dictionary<string, object> response = new Dictionary<string, object>();
        string url = "https://odin-game-7edee-default-rtdb.firebaseio.com/" + collection + "/" + id + ".json";
        string body = "";
        /* You may need to add header(s) */
        // request.SetRequestHeader("Content-Type", "application/json");
        UnityWebRequest www = UnityWebRequest.Post(url, body, "application/json");
        www.method = "DELETE";
        yield return www.SendWebRequest();
        //Debug.Log(www.result);
        // TODO: se puede quedar trabado por falta de conexion 
        if (www.result != UnityWebRequest.Result.Success)
        {
            //Debug.Log(www.error);
            response.Add("result", false);
            response.Add("message", www.downloadHandler.text);
        }
        else
        {
            //JsonUtility.FromJson
            response.Add("result", true);
            response.Add("data", www.downloadHandler.text);

        }
        callback(response);

    }

}
