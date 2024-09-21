import json
import os
import time
import requests

GLOBAL_SAVED_DATA = {}
#例子: {'订单编号': 'Server酱通知的key(可为空)'}
QUERYID_AND_NOTIFYID = {'': ''}
#轮询时间
TIMER_SLEEP = 5

def main_handler(event, context):
    if TIMER_SLEEP < 1:
        print('轮询时间至少1秒!!!')
        exit()
    if not QUERYID_AND_NOTIFYID:
        print('未填入关键信息, 请右键编辑py脚本的关键信息!!!')
        input('按回车键退出本程序...')
        exit()
    for key, value in QUERYID_AND_NOTIFYID.items():
        if (len(key) < 15):
            print('未填入关键信息, 请右键编辑py脚本的关键信息!!!')
            input('按回车键退出本程序...')
            exit()
    while True:
        for key_id in QUERYID_AND_NOTIFYID.keys():
            get_data(key_id)
        time.sleep(TIMER_SLEEP)
    
def get_data(order_id):
    post_data = {'wxappAid': '3086825', 'wxappId': '101', 'itemId': '103', 'contentList': '[{"key":"v2","value":"' + order_id + '"}]'}
    try:
        response = requests.post('https://i.qz.fkw.com/appAjax/wxAppConnectionQuery.jsp?cmd=search', data=post_data)
        if (response.status_code == 200):
            response = response.text.replace('\r\n', '')
            zt_data = json.loads(response)
            if (zt_data["success"] and len(zt_data['queryDataList']) > 0):
                data_do(zt_data['queryItem']['queryCondition'], zt_data['queryDataList'][0]['content'])
    except Exception as e:
        print(e)
        
def data_do(display, info):
    log_dict = {}
    for item in display:
        log_dict[item["n"]] = info[item["k"]]
    print(json.dumps(log_dict, ensure_ascii=False))
    verfiy_data(log_dict)
        
def verfiy_data(data):
    last_data = get_temp(data["订单编号"])
    if last_data:
        if data["数据更新时间"] != last_data["数据更新时间"]:
            save_temp(data)
            print("[" + format_time() + "] 状态发生更新!!!")
            if QUERYID_AND_NOTIFYID[data["订单编号"]]:
                notify_wechat(QUERYID_AND_NOTIFYID[data["订单编号"]], data, last_data["进度"])
        else:
            print("[" + format_time() + "] 无更新!!!")
    else:
        save_temp(data)
    
def notify_wechat(notify_key, dt, last_dt):
    post_data = {'title': '头壳{} {}辣!'.format(dt["批次"], dt["进度"]), 'desp': '## {}的头壳记录更新辣!\n***\n- 批次: {}\n- 状态: {} => {}\n- 时间: {}\n- 信息: {}'.format(dt["ID"], dt["批次"], last_dt, dt["进度"], dt["数据更新时间"], dt["OvO"])}
    response = requests.post('https://sctapi.ftqq.com/' + str(notify_key) + '.send', data=post_data)
    if (response.status_code == 200):
        print("[" + format_time() + '] 状态更新推送到微信成功.')
        
def format_time():
    return time.strftime('%Y-%m-%d %H:%M:%S', time.localtime(time.time()))
    
def save_temp(info_data):
    #info_file = 'last_info.txt'
    #with open(info_file, 'w') as f:
    #    json.dump(info_data, f)
	global GLOBAL_SAVED_DATA
	GLOBAL_SAVED_DATA[info_data["订单编号"]] = info_data
	

def get_temp(QueryID):
    #info_file = 'last_info.txt'
    #if os.path.exists(info_file):
    #    with open(info_file, 'r') as f:
    #        try:
    #            return json.load(f)
    #        except Exception as e:
    #            return None
    #return None
	if QueryID in GLOBAL_SAVED_DATA:
		return GLOBAL_SAVED_DATA[QueryID]
	return None
    
    
if __name__ == '__main__':
    main_handler(None, None)