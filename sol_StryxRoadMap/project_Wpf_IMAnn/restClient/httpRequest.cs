using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Wpf_IMAnn.Utils;
using Wpf_IMAnn.Models;

namespace Wpf_IMAnn.restClient
{
    class httpRequest
    {
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
            return new List<ImageNodeModel>();
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
        public AnnoShapeModel addAnnotation(AnnoShapeModel annoShapeModel)
        {
            return annoShapeModel;
        }

        //GET /api/v1/annotation/multiple/:image
        public List<AnnoShapeModel> getAnnotations(int imageId)
        {
            return new List<AnnoShapeModel>();
        }

        //PUT /api/v1/annotation/one/:image
        public JObject modifyAnnotation(AnnoShapeModel annoShapeModel)
        {
            return new JObject();
        }

        //DELETE /api/v1/annotation/one/:image
        //outout : 성공 - 0 / bad request - 1 / server 문제 - 2 
        public int deleteAnnotation(AnnoShapeModel annoShapeModel)
        {
            return 0;
        }
        


    }
}
