<?php
//人脸识别考勤系统
//考勤设备信息接口  http://192.168.11.42:81/api/query/?table=device&&key=7234rr9rfgnr4gwrdwbyxywatjx5rismmaqj8wjsayhf
//考勤记录  get  http://192.168.11.42:81/api/query/?table=checkinout&key=&startdate=20181220&enddate=20181230
set_time_limit(999999999999);//运行时间，防止自动停止

$ip = "http://127.0.0.1:81";
$api_sign = '/api/v2/transaction/get/';
$key = "b38evirzj4iirgylqwuim_dm001dyxf8zv_qrgyl2d11";
$machine = array( //编号和序列号的关系
    0=>array('no'=>'01','sn'=>'173708520020'),//大机器
    1=>array('no'=>'02','sn'=>'173708520022'),//大机器
    2=>array('no'=>'03','sn'=>'CEXB185260001'),
    3=>array('no'=>'04','sn'=>'CEXB185260002'),
    4=>array('no'=>'05','sn'=>'CEXB185260003'),
    5=>array('no'=>'06','sn'=>'CEXB185260004'),
    6=>array('no'=>'07','sn'=>'CEXB185260005'),
    7=>array('no'=>'08','sn'=>'CEXB185260006'),
    8=>array('no'=>'09','sn'=>'CEXB185260007'),
    9=>array('no'=>'10','sn'=>'CEXB185260008'),
    10=>array('no'=>'11','sn'=>'CEXB185260009'),
    11=>array('no'=>'12','sn'=>'CEXB185260010'),
	12=>array('no'=>'13','sn'=>'CEXB191260001'),
    13=>array('no'=>'14','sn'=>'CEXB191260002'),
    14=>array('no'=>'15','sn'=>'CEXB191260003'),
    15=>array('no'=>'16','sn'=>'CEXB191260004'),
    16=>array('no'=>'17','sn'=>'CEXB191260005'),
    17=>array('no'=>'18','sn'=>'CEXB191260006'),
    18=>array('no'=>'19','sn'=>'CEXB191260007'),
    19=>array('no'=>'20','sn'=>'CEXB191260008'),
);

$a = true;
$i = 0;
$j = 0;
$success = ',';
do{

    //获取当天考勤总数
    $start = date('Ymd',time());
    $end = date('Ymd',time()+24*3600);
    $url = $ip."/api/query/?table=checkinout&key=".$key."&startdate=".$start."&enddate=".$end;
    $result = curl_get($url);
    $result = json_decode($result,true);
    $items = $result['data']['items']; //考勤数据

    if(!empty($items)){
		//file_put_contents('sign-abc.txt', print_r($items, true). "\r\n",FILE_APPEND);
		file_put_contents('sign.txt', 'Start ----------------------------- '. "\r\n",FILE_APPEND);
        foreach ($items as $k=>$v){
            $get = 0;//是否请求接口,0不请求，1请求
            $j++;
            $date = date('Y-m-d H:i:s',time());
            $no = '01';
            $userId = $v['pin'];//会员编号
            $sn = $v['sn'];//设备编码
            $checktime = $v['checktime'];//签到时间
            $checktime = strtotime($checktime);
            
			if(($k>=$i)&&(!strstr($success,','.$userId.$checktime))){
                $get=1;//请求接口
            }
			
            if($get==1){
                $i = $k;// 数组最大值
                foreach ($machine as $k1=>$v1){
                    if($sn==$v1['sn']){
                        $no = $v1['no'];//设备编码对应的编号
						break;
                    }
                }
                //调用签到接口
//                $sign_url = "http://hkzx.quyou.net/m/sign.aspx?token=";//token是委员加密后的字符
                $sign_url = "http://117.184.33.144/m/sign.aspx?token=";//token是委员加密后的字符
                $sign_token = getToken($userId,$no,$checktime);
                $sign_url = $sign_url.$sign_token;
                $sign_result = curl_get($sign_url);
                if($sign_result=='签到成功'){
                    file_put_contents('sign.txt', $j . ' -- no：' . $no . ' time：' . $date .' 委员：'.$userId.' 签到时间：'.$checktime.'签到成功：'.$sign_result."\r\n",FILE_APPEND);
                    $success.=",".$userId.$checktime;
                }else{
                    $message = '签到失败';  // 委员编号错误！ 您已签到了！
                    file_put_contents('sign_error.txt', $j . ' -- no：' . $no . ' time：' . $date .' 委员：'.$userId.' 签到时间：' . $checktime . ' 签到失败：'.$sign_result."\r\n",FILE_APPEND);
                }
            }else{
                // file_put_contents('sign.txt',$date .'委员：'.$userId.'已经请求过了'.$k."\r\n",FILE_APPEND);
            }
        }
       
			die;
    }else{
		file_put_contents('sign-def.txt', "数据为空" ."\r\n",FILE_APPEND);
        $date = date('Y-m-d H:i:s',time());
    }

	sleep(2);


}while($a);


//接口加密
function getToken($userId,$no,$checktime){
    $no = strval($no);
    $pagram = $no.'='.$userId;
    if(!empty($checktime)){
        $pagram.='='.$checktime;
    }
    exec('D:\www\ceshi\ConsoleDes.exe '.$pagram,$info);//参数拼在exe后面，$info取得结果
    return $info[0];
}


function curl_post($url,$data){
    $ch = curl_init();//初始化curl
    curl_setopt($ch, CURLOPT_URL,$url);//抓取指定网页
    curl_setopt($ch, CURLOPT_HEADER, 0);//设置header
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);//要求结果为字符串且输出到屏幕上
    curl_setopt($ch, CURLOPT_POST, 1);//post提交方式
    curl_setopt($ch, CURLOPT_POSTFIELDS, $data);
    $result = curl_exec($ch);//运行curl
    curl_close($ch);
    $result = json_decode($result,true);
    return $result;
}
function curl_get($url){
    //初始化
    $ch = curl_init();
    //设置选项，包括URL
    curl_setopt($ch, CURLOPT_URL, $url);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
    curl_setopt($ch, CURLOPT_HEADER, 0);
    //执行并获取HTML文档内容
    $output = curl_exec($ch);
    //释放curl句柄
    curl_close($ch);
    //打印获得的数据
    return $output;
}

