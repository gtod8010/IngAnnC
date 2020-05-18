using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Wpf_IMAnn_Server.Utils;
using Wpf_IMAnn_Server.Models;
using Wpf_IMAnn_Server.ViewModels;


namespace Wpf_IMAnn_Server.restClient
{
    class HttpRequestClient
    {
        private static HttpRequestClient requstClient;
        private string host;
        private HttpClient client;

        protected HttpRequestClient()
        {
            host = "http://10.0.0.107";
            client = new HttpClient();
            client.BaseAddress = new Uri(host);
            client.DefaultRequestHeaders.Add("accept", "application/json; charset=utf-8");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
        }

        public static HttpRequestClient Instance()
        {
            if (requstClient == null)
            {
                requstClient = new HttpRequestClient();
            }
            return requstClient;
        }

        /*
        GET /api/v1/image/urls/:num/:token -> 야매 토큰 ip 점때고 숫자만 넘긴다
        POST /api/v1/annotation/one/:image - annotation을 등록
        GET /api/v1/annotation/multiple/:image - 등록된 annotation을 가져옴
        PUT /api/v1/annotation/one/:image - 등록된 annotation 수정
        DELETE /api/v1/annotation/one/:image - 등록된 annotation 삭제
        */


        // input : 이미지 개수
        // output : 이미지 url (ex. "http://10.0.0.104:3000/download/677579")
        // GET /api/v1/image/urls/:num/:token -> 야매 토큰 ip 점때고 숫자만 넘긴다
        public List<ImageNodeModel> getImageUrl(int num)
        {
            var images = new List<ImageNodeModel>();
            string responseBody;
            HttpResponseMessage response;

            var entrypoint = String.Format("api/v1/image/urls/{0}", num);
            try
            {
                response = client.GetAsync(entrypoint).Result;
                response.EnsureSuccessStatusCode();

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(String.Format("\nThe request : {0}is fail!", entrypoint));
                Console.WriteLine("Message :{0} ", e.Message);
                return images;
            }

            try
            {
                responseBody = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception e)
            {
                Console.WriteLine("response body is not readable");
                return images;
            }

            JObject resJsonObj = JObject.Parse(responseBody);
            JArray imageJsonList = resJsonObj.SelectToken("images") as JArray;
            foreach (var imageJsonObj in imageJsonList)
            {
                int imageId = imageJsonObj.Value<int>("id");
                int pixelWidth = imageJsonObj.Value<int>("width");
                int pixelHeight = imageJsonObj.Value<int>("height");
                string imageUrl = @"" + imageJsonObj.Value<string>("img_url");

                images.Add(new ImageNodeModel(imageId, pixelWidth, pixelHeight, imageUrl));
            }

            return images;
        }

        // input : curl -X GET /api/v1/image/urls/5
        // output : 
        /*
         * (
    'status' : 'success',
    'images' : [{
        'pvrid' : 1201101012345, 'width' : 13000, 'height' : 6500,
        'img_url' : 'stryx.iptime.org/imagepath/image1.jpg'
        },{
        'pvrid' : 1201101012346, 'width' : 13000, 'height' : 6500,
        'img_url' : 'stryx.iptime.org/imagepath/image2.jpg'
        },{
        'pvrid' : 1201101012347, 'width' : 13000, 'height' : 6500,
        'img_url' : 'stryx.iptime.org/imagepath/image3.jpg'
        },{
        'pvrid' : 1201101012348, 'width' : 13000, 'height' : 6500,
        'img_url' : 'stryx.iptime.org/imagepath/image4.jpg'
        },{
        'pvrid' : 1201101012349, 'width' : 13000, 'height' : 6500,
        'img_url' : 'stryx.iptime.org/imagepath/image5.jpg'
        }
    ]
}
         */

        //POST /api/v1/annotation/one/:image
        //outout : 성공 - 0 / bad request - 1 / server 문제 - 2 / 기타문제 - 3
        public int addAnnotation(AnnoShapeModel annoShapeModel)
        {            
            string responseBody;
            HttpResponseMessage response;


            var annotationJson = new JObject();
            annotationJson.Add("shape_type", shapeTypeEnumToSring(annoShapeModel.shapetype));
            annotationJson.Add("class", 0);
            annotationJson.Add("recognition", Convert.ToInt32(annoShapeModel.signfield));
            if (annoShapeModel.memo != null && !string.Equals(annoShapeModel.memo, ""))
            {
                annotationJson.Add("memo", annoShapeModel.memo);
            }


            switch (annoShapeModel.shapetype)
            {
                case ShapeType.point:
                    annotationJson.Add("point", getPointJson(annoShapeModel));
                    break;
                case ShapeType.boundingbox:
                    annotationJson.Add("bbox", getBboxJson(annoShapeModel));
                    break;
                case ShapeType.line:
                    annotationJson.Add("polyline", getPolyJson(annoShapeModel));
                    break;
                case ShapeType.polygon:
                    annotationJson.Add("polygon", getPolyJson(annoShapeModel));
                    break;
            }

            string entrypoint = String.Format("api/v1/annotation/one/{0}", annoShapeModel.imageid);
            try
            {
                response = client.PostAsJsonAsync(entrypoint, annotationJson).Result;
                response.EnsureSuccessStatusCode(); // 오류 코드를 던집니다.
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(String.Format("\nThe request : {0}is fail!", entrypoint));
                Console.WriteLine("Message :{0} ", e.Message);
                return 2;
            }

            try
            {
                responseBody = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception e)
            {
                Console.WriteLine("response body is not readable");
                return 3;
            }

            JObject resJsonObj = JObject.Parse(responseBody);
            int shapeId = resJsonObj.Value<int>("shape_id");

            annoShapeModel.uniqueShapeid = shapeId;           

            return 0;
        }

        //GET /api/v1/annotation/multiple/:image
        public List<AnnoShapeModel> getAnnotations(int imageId)
        {
            List<AnnoShapeModel> annoShapes = new List<AnnoShapeModel>();

            string responseBody;
            HttpResponseMessage response;

            var entrypoint = String.Format("api/v1/annotation/multiple/{0}?annotations=[\"all\"]", imageId);
            try
            {
                response = client.GetAsync(entrypoint).Result;
                response.EnsureSuccessStatusCode();

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(String.Format("\nThe request : {0}is fail!", entrypoint));
                Console.WriteLine("Message :{0} ", e.Message);
                return annoShapes;
            }

            try
            {
                responseBody = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception e)
            {
                Console.WriteLine("response body is not readable");
                return annoShapes;
            }


            JObject resJsonObj = JObject.Parse(responseBody);
            var annotationJsonList = resJsonObj.SelectToken("annotations");
            if (annotationJsonList == null)
            {
                return annoShapes;
            }
            foreach (var annotationJsonObj in annotationJsonList)
            {
                List<double> cols = new List<double>();
                List<double> rows = new List<double>();
                int shapeId = annotationJsonObj.Value<int>("shape_id");
                string typeString = annotationJsonObj.Value<string>("shape_type");
                ShapeType shapeType = stringToShapeType(typeString);
                int shapeClass = annotationJsonObj.Value<int>("class");
                int recognition = annotationJsonObj.Value<int>("recognition");
                var signField = mIdentifySignField(recognition);
                var shapeCoord = annotationJsonObj.SelectToken(typeString);

                AnnoShapeModel annoShapeModel = new AnnoShapeModel(shapeType);
                annoShapeModel.uniqueShapeid = shapeId;
                annoShapeModel.imageid = imageId;
                annoShapeModel.shapetype = shapeType;
                annoShapeModel.signfield = signField;
                annoShapeModel.col = cols;
                annoShapeModel.row = rows;
                assignColRowFromshapeCoordsJson(shapeCoord, annoShapeModel);

                if (annotationJsonObj.SelectToken("memo") != null)
                {
                    annoShapeModel.memo = annotationJsonObj.Value<string>("memo");
                }

                annoShapes.Add(annoShapeModel);
            }
            return annoShapes;
        }

        //PUT /api/v1/annotation/one/:image
        //outout : 성공 - 0 / bad request - 1 / server 문제 - 2 / 기타문제 - 3
        public int modifyAnnotation(AnnoShapeModel annoShapeModel)
        {

            string responseBody;
            HttpResponseMessage response;


            var annotationJson = new JObject();
            annotationJson.Add("shape_id", annoShapeModel.uniqueShapeid);
            annotationJson.Add("shape_type", shapeTypeEnumToSring(annoShapeModel.shapetype));
            annotationJson.Add("class", 0);
            annotationJson.Add("recognition", Convert.ToInt32(annoShapeModel.signfield));
            if (annoShapeModel.memo != null && !string.Equals(annoShapeModel.memo, ""))
            {
                annotationJson.Add("memo", annoShapeModel.memo);
            }


            switch (annoShapeModel.shapetype)
            {
                case ShapeType.point:
                    annotationJson.Add("point", getPointJson(annoShapeModel));
                    break;
                case ShapeType.boundingbox:
                    annotationJson.Add("bbox", getBboxJson(annoShapeModel));
                    break;
                case ShapeType.line:
                    annotationJson.Add("polyline", getPolyJson(annoShapeModel));
                    break;
                case ShapeType.polygon:
                    annotationJson.Add("polygon", getPolyJson(annoShapeModel));
                    break;
            }

            string entrypoint = String.Format("/api/v1/annotation/one/{0}", annoShapeModel.imageid);

            try
            {
                response = client.PutAsJsonAsync(entrypoint, annotationJson).Result;
                response.EnsureSuccessStatusCode(); // 오류 코드를 던집니다.

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(String.Format("\nThe request : {0}is fail!", entrypoint));
                Console.WriteLine("Message :{0} ", e.Message);
                return 2;
            }

            try
            {
                responseBody = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception e)
            {
                Console.WriteLine("response body is not readable");
                return 3;
            }


            return 0;
        }

        //DELETE /api/v1/annotation/one/:image
        //outout : 성공 - 0 / bad request - 1 / server 문제 - 2 / 기타문제 - 3
        public int deleteAnnotation(AnnoShapeModel annoShapeModel)
        {
            string responseBody;
            HttpResponseMessage response;

            var entrypoint = String.Format("/api/v1/annotation/one/{0}/{1}", annoShapeModel.imageid, annoShapeModel.uniqueShapeid);
            try
            {
                response = client.DeleteAsync(entrypoint).Result;
                response.EnsureSuccessStatusCode();

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(String.Format("\nThe request : {0}is fail!", entrypoint));
                Console.WriteLine("Message :{0} ", e.Message);
                return 2;
            }

            try
            {
                responseBody = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception e)
            {
                Console.WriteLine("response body is not readable");
                return 3;
            }
            return 0;
        }

        //PUT /api/v1/image/commit/:image',
        //outout : 성공 - 0 / bad request - 1 / server 문제 - 2 / 기타문제 - 3
        // 툴 꺼질 때 실행 
        public int commitImage(ImageNodeModel image)
        {
            string responseBody;
            HttpResponseMessage response;

            var entrypoint = String.Format("/api/v1/image/commit/{0}", image.imageid);
            try
            {
                response = client.PutAsync(entrypoint, null).Result;
                response.EnsureSuccessStatusCode();

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(String.Format("\nThe request : {0}is fail!", entrypoint));
                Console.WriteLine("Message :{0} ", e.Message);
                return 2;
            }

            try
            {
                responseBody = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception e)
            {
                Console.WriteLine("response body is not readable");
                return 3;
            }

            return 0;
        }

        private string shapeTypeEnumToSring(ShapeType enumType)
        {
            switch (enumType)
            {
                case ShapeType.none:
                    return "none";
                case ShapeType.point:
                    return "point";
                case ShapeType.boundingbox:
                    return "bbox";
                case ShapeType.line:
                    return "polyline";
                case ShapeType.polygon:
                    return "polygon";
            }
            return "none";
        }

        private ShapeType stringToShapeType(string stringType)
        {
            switch (stringType)
            {
                case "none":
                    return ShapeType.none;
                case "point":
                    return ShapeType.point;
                case "bbox":
                    return ShapeType.boundingbox;
                case "polyline":
                    return ShapeType.line;
                case "polygon":
                    return ShapeType.polygon;
            }
            return ShapeType.none;
        }

        private JObject getPointJson(AnnoShapeModel annoShapeModel)
        {
            var point = new JObject();

            point.Add("x", annoShapeModel.col[0]);
            point.Add("y", annoShapeModel.row[0]);

            return point;
        }

        private JArray getBboxJson(AnnoShapeModel annoShapeModel)
        {
            var pointList = new JArray();
            var point1 = new JObject();
            var point2 = new JObject();
            var RectSize = new JObject();

            point1.Add("x", annoShapeModel.col[0]);
            point1.Add("y", annoShapeModel.row[0]);

            point2.Add("x", annoShapeModel.col[1]);
            point2.Add("y", annoShapeModel.row[1]);

            RectSize.Add("width", annoShapeModel.width);
            RectSize.Add("height", annoShapeModel.height);

            pointList.Add(point1);
            pointList.Add(point2);
            pointList.Add(RectSize);
            return pointList;
        }

        private JArray getPolyJson(AnnoShapeModel annoShapeModel)
        {
            var poly = new JArray();

            foreach (var item in annoShapeModel.col.Select((value, index) => new { Value = value, Index = index }))
            {
                double x = item.Value;
                double y = annoShapeModel.row[item.Index];

                var singlePoint = new JObject();

                singlePoint.Add("x", x);
                singlePoint.Add("y", y);

                poly.Add(singlePoint);
            }

            return poly;
        }

        public object mIdentifySignField(int signFieldId)
        {
            if (Enum.IsDefined(typeof(SignField_Caution), signFieldId))
                return (SignField_Caution)Enum.Parse(typeof(SignField_Caution), signFieldId.ToString());
            else if (Enum.IsDefined(typeof(SignField_Instruction), signFieldId))
                return (SignField_Instruction)Enum.Parse(typeof(SignField_Instruction), signFieldId.ToString());
            else if (Enum.IsDefined(typeof(SignField_Regulation), signFieldId))
                return (SignField_Regulation)Enum.Parse(typeof(SignField_Regulation), signFieldId.ToString());
            else if (Enum.IsDefined(typeof(LightField), signFieldId))
                return (LightField)Enum.Parse(typeof(LightField), signFieldId.ToString());
            else if (Enum.IsDefined(typeof(SurfaceMark), signFieldId))
                return (SurfaceMark)Enum.Parse(typeof(SurfaceMark), signFieldId.ToString());
            else if (Enum.IsDefined(typeof(SignField_ETC), signFieldId))
                return (SignField_ETC)Enum.Parse(typeof(SignField_ETC), signFieldId.ToString());
            else if (Enum.IsDefined(typeof(SignField_Assistant), signFieldId))
                return (SignField_Assistant)Enum.Parse(typeof(SignField_Assistant), signFieldId.ToString());
            else if (Enum.IsDefined(typeof(Construction), signFieldId))
                return (Construction)Enum.Parse(typeof(Construction), signFieldId.ToString());
            else if (Enum.IsDefined(typeof(Human), signFieldId))
                return (Human)Enum.Parse(typeof(Human), signFieldId.ToString());
            else if (Enum.IsDefined(typeof(Car), signFieldId))
                return (Car)Enum.Parse(typeof(Car), signFieldId.ToString());
            else
                return SignField.미할당;
        }

        public void assignColRowFromshapeCoordsJson(JToken coords, AnnoShapeModel annoShapeModel)
        {
            switch (annoShapeModel.shapetype)
            {
                case ShapeType.none:
                    break;
                case ShapeType.point:
                    annoShapeModel.col.Add(coords.Value<double>("x"));
                    annoShapeModel.row.Add(coords.Value<double>("y"));
                    break;
                case ShapeType.boundingbox:
                    JArray bBoxCoords = coords as JArray;
                    annoShapeModel.col.Add(coords[0].Value<double>("x"));
                    annoShapeModel.row.Add(coords[0].Value<double>("y"));
                    annoShapeModel.col.Add(coords[1].Value<double>("x"));
                    annoShapeModel.row.Add(coords[1].Value<double>("y"));
                    annoShapeModel.width = coords[2].Value<double>("width");
                    annoShapeModel.height = coords[2].Value<double>("height");
                    break;
                case ShapeType.line:
                case ShapeType.polygon:
                    JArray points = coords as JArray;
                    foreach (var point in points)
                    {
                        annoShapeModel.col.Add(point.Value<double>("x"));
                        annoShapeModel.row.Add(point.Value<double>("y"));
                    }
                    break;
            }
            return;
        }

        public int loadImageIntoWorkplace(ImageNodeModel image)
        {


            HttpResponseMessage response;


            var entrypoint = String.Format("/api/v1/image/workplace/{0}", image.imageid);
            try
            {
                response = client.PutAsync(entrypoint, null).Result;
                response.EnsureSuccessStatusCode();


            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(String.Format("\nThe request : {0}is fail!", entrypoint));
                Console.WriteLine("Message :{0} ", e.Message);
                return 2;
            }


            return 0;
        }

    }
}
