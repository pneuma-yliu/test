using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCRMAPI
{
    class Program
    {
        public static void Main(string[] args)
        {

        }

        private ApiStatus UpdateEntitiesAsync(string entityPluralName, Dictionary<string, object> listUpdateData)
        {
            var apiResult = new ApiStatus();
            if (_accessToken.Equals("ERROR"))
            {
                apiResult.Message = "Could not get CRM Access Token";
                return apiResult;
            }

            /* sample:
            --batch_AAA123  
            Content-Type: multipart/mixed;boundary=changeset_BBB456  

            --changeset_BBB456  
            Content-Type: application/http  
            Content-Transfer-Encoding:binary  
            Content-ID: 1  

            PATCH https://cliqstudiosdev.api.crm.dynamics.com/api/data/v9.1/contacts(f6a19be5-b1eb-e811-a968-000d3a199997) HTTP/1.1
            Content-Type: application/json;type=entry  

            { "telephone1":"6464646464" }  
            --changeset_BBB456  
            Content-Type: application/http  
            Content-Transfer-Encoding:binary  
            Content-ID: 2  

            PATCH https://cliqstudiosdev.api.crm.dynamics.com/api/data/v9.1/contacts(10989775-b194-e811-a961-000d3a1993e0) HTTP/1.1
            Content-Type: application/json;type=entry  

            { "email":"yml,test@cliq.com" }  
            --changeset_BBB456--  
            --batch_AAA123--
            */

            try
            {
                StringBuilder batchContent = new StringBuilder();
                string batch = $"batch_{Guid.NewGuid().ToString()}";
                string changeset = $"changeset_{Guid.NewGuid().ToString()}";

                /*batch body*/
                batchContent.AppendLine($"--{batch}");
                batchContent.AppendLine($"Content-Type: multipart/mixed;boundary={changeset}");
                batchContent.AppendLine($"");

                int content_id = 0;
                foreach (var item in listUpdateData)
                {
                    content_id += 1;
                    string entityId = item.Key;
                    object entityData = item.Value;
                    batchContent.AppendLine($"--{changeset}");
                    batchContent.AppendLine($"Content-Type: application/http");
                    batchContent.AppendLine($"Content-Transfer-Encoding:binary");
                    batchContent.AppendLine($"Content-ID: {content_id}");
                    batchContent.AppendLine($"");
                    batchContent.AppendLine($"PATCH {_apiPath}/{entityPluralName}({entityId}) HTTP/1.1");
                    batchContent.AppendLine($"Content-Type: application/json;type=entry");
                    batchContent.AppendLine($"");
                    batchContent.AppendLine(JsonConvert.SerializeObject(entityData, Formatting.Indented));
                }

                batchContent.AppendLine($"--{changeset}--");
                batchContent.AppendLine($"--{batch}--");

                /*send batch api*/
                var requestUri = $"{_apiPath}/$batch";
                //var response = await _httpClient.PostAsync(requestUri, new StringContent(batchContent.ToString(), Encoding.UTF8, $"multipart/mixed;boundary={batch}"));
                MultipartContent content = new MultipartContent("mixed", batch);
                content.Headers.Remove("Content-Type");
                content.Headers.TryAddWithoutValidation("Content-Type", $"multipart/mixed;boundary={batch}");
                content.Add(new StringContent(batchContent.ToString(), Encoding.UTF8));
                var response = await _httpClient.PostAsync(requestUri, content);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    apiResult.Status = true;
                    apiResult.Message = "Success";
                }
                else
                {
                    apiResult.Message = response.StatusCode.ToString();
                }
            }
            catch (Exception ex)
            {
                apiResult.Status = false;
                apiResult.Message = ex.Message;
                Console.WriteLine($"UpdateEntities Error : {ex.Message}");
            }

            return apiResult;
        }

    }
}
