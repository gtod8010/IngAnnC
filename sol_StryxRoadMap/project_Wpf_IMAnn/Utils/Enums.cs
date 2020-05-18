﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Wpf_IMAnn.Utils
{
  
    public enum ShapeType // Annotation shape
    {
        none = 0,
        line = 1,
        polygon = 2,
        boundingbox = 3,
        point = 4,
    }

    public enum SignField
    {
        미할당 = 0,
    }

    public enum ETC
    {
        보조표지 = 499,
        킬로포스트 = 498,
        기타_노면 = 599,
        기타_신호 = 99
    }

    public enum SignField_Caution // Field of safety signs
    {
        미할당 =0,
        플러스자형교차로 = 101,
        T자형교차로 = 102,
        Y자형교차로 = 103,
        ㅏ자형교차로 = 104,
        ㅓ자형교차로 = 105,
        우선도로 = 106,
        우합류도로 = 107,
        좌합류도로 = 108,
        회전형교차로 = 109,        
        철길건널목 = 110,
        우로굽은도로 = 111,
        좌로굽은도로 = 112,
        우좌로이중굽은도로 = 113,
        좌우로이중굽은도로 = 114,
        양방향통행 = 115,
        오르막경사 = 116,
        내리막경사 = 117,
        도로폭이좁아짐 = 118,
        우측차로없어짐 = 119,
        좌측차로없어짐 = 120,
        우측방통행 = 121,
        양측방통행 = 122,
        중앙분리대시작 = 123,
        중앙분리대끝남 = 124,
        신호기 = 125,
        미끄러운도로 = 126,
        강변도로 = 127,
        노면고르지못함 = 128,
        과속방지턱 = 129,
        낙석도로 = 130,
        횡단보도_주의 = 132,
        어린이보호_주의 = 133,
        자전거 = 134,
        도로공사중 = 135,
        비행기 =136,
        횡풍  =137,
        터널 = 138,
        교량 = 1382,
        야생동물보호 = 139,
        위험 = 140,
        상습정체구간 = 141,
    }

    public enum SignField_Regulation
    {
        미할당 = 0,
        통행금지 = 201,
        자동차_통행금지 = 202,
        화물자동차_통행금지 = 203,
        승합자동차_통행금지 = 204,
        이륜자동차_및_원동기장치_자전거_통행금지 = 205,
        자동차_이륜자동차_및_원동기장치_자전거_통행 = 206,
        경운기트랙터_및_손수레_통행금지 = 207,
        자전거_통행금지 = 210,
        진입금지 = 211,
        직진금지 = 212,
        우회전금지 = 213,
        좌회전금지 = 214,
        유턴금지 = 216,
        앞지르기금지 = 217,
        정차주차금지 = 218,
        주차금지 = 219,
        차중량제한 = 220,
        차높이제한 = 221,
        차폭제한 = 222,
        차간거리확보 = 223,
        최고속도제한 = 224,
        최저속도제한 = 225,
        서행 = 226,
        일시정지 = 227,
        양보 = 228,
        보행자_통행금지 = 230,
        위험물적재차량_통행금지 = 231,
    }

    public enum SignField_Instruction
    {
        미할당 = 0,
        자동차전용도로 = 301,
        자전거전용도로 = 302,
        자전거_보행자_겸용도로 = 303,
        회전교차로 = 304,
        직진 = 305,
        우회전 = 306,
        좌회전 = 307,
        직진_및_우회전 = 308,
        직진_및_좌회전 = 309,
        좌회전_및_유턴 = 3092,
        좌우회전 = 310,
        유턴 = 311,
        양측방통행 = 312,
        우측면통행 = 313,
        좌측면통행 = 314,
        진행방향별_통행구분 = 315,
        우회로 = 316,
        자전거_및_보행자_통행구분 = 317,
        자전거전용차로 = 318,
        주차장 = 319,
        자전거_주차장 =320,
        보행자_전용도로 = 321,
        횡단보도_지시 = 322,
        노인보호 = 323,
        어린이보호_지시 = 324,
        장애인보호구역안 = 3242,
        자전거횡단도 = 325,
        일방통행_우측 = 326,
        일방통행_좌측 = 327,
        일방통행_직진 = 328,
        비보호좌회전 = 329,
        버스전용차로 = 330,
        다인승차량전용차로 = 331,
        통행우선 = 332,
        자전거나란히통행허용 = 333,
    }

    public enum LightField
    {
        미할당 =0,
        차량횡형_삼색등 =1,
        차량횡형_사색등A = 2,
        차량횡형_사색등B =3,
        차량횡형_화살표삼색등=4,
        차량종형_삼색등=5,
        차량종형_화살표삼색등=6,
        차량종형_사색등=7,
        버스삼색등=8,
        가변형_가변등=9,
        경보형_가변등=10,
        보행등=11,
        자전거종형_삼색등=12,
        자전거종형_이색등=13,
        차량보조등_종형삼색등=14,
        차량보조등_종형사색등=15
    }

    public enum SufaceMark_Line
    {
        미할당=0,
        중앙선=501,
        가변차선=5011,
        유턴구역선=502,
        차선=503,
        버스전용차선=504,
        길가장자리구역선=505,
        진로변경제한선=506,
        주차금지선=515,
        유도선=525,
        정지선=530,
        안전지대=531,
        자전거도로=535
    }

    public enum SurfaceMark
    {
        미할당=0,

        //중앙선 = 501,
        //가변차선 = 5011,
        //유턴구역선 = 502,
        //차선 = 503,
        //버스전용차선 = 504,
        //길가장자리구역선 = 505,
        //진로변경제한선 = 506,
        //주차금지선 = 515,
        //정차금지대 =524,
        //유도선 = 525,
        정지선 = 530,
        안전지대 = 531,
        횡단보도=532,
        고원식횡단보도=533,
        자전거횡단보도=534,
        자전거도로 = 535,
        직진=5371,
        좌회전=5372,
        우회전=5373,
        좌우회전=5374,
        전방향=5379,
        직진_및_좌회전=5381,
        직진_및_우회전=5382,
        직진_및_유턴=5383,
        유턴=5391,
        좌회전_및_유턴=5392,
        차로변경_좌로합류=5431,
        차로변경_우로합류=5432,
        오르막경사면=544
    }
}