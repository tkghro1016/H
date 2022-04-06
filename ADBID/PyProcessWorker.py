###############################################################################################
#
#
# -- Python Module Import 및 Arguments 선언
#
#
###############################################################################################

import time
import requests
import hashlib
import hmac
import base64
import jsonpickle
import os
import sys
import pandas as pd
import numpy as np


BASE_URL = sys.argv[1]
API_KEY = sys.argv[2]
SECRET_KEY = sys.argv[3]
CUSTOMER_ID = sys.argv[4]
kwd_data = sys.argv[5]



###############################################################################################
#
#
# -- Naver 광고 API Get Method 선언
#
#
###############################################################################################
def get_header(method, uri, api_key, secret_key, customer_id):
    timestamp = str(round(time.time() * 1000))
    message = "{}.{}.{}".format(timestamp, method, uri)
    hash = hmac.new(bytes(secret_key, "utf-8"), bytes(message, "utf-8"), hashlib.sha256)
    hash.hexdigest()
    signature = base64.b64encode(hash.digest())
    return {'Content-Type': 'application/json; charset=UTF-8', 'X-Timestamp': timestamp, 'X-API-KEY': api_key,
            'X-Customer': str(customer_id), 'X-Signature': signature}

def get_campaign_list(base_url, api_key, secret_key, customer_id):
    uri = '/ncc/campaigns'
    method = 'GET'
    r = requests.get(base_url + uri, headers=get_header(method, uri, api_key, secret_key, customer_id))

    return r.json()

def get_adgroup_list(base_url, api_key, secret_key, customer_id):
    uri = '/ncc/adgroups'
    method = 'GET'
    r = requests.get(base_url + uri, headers=get_header(method, uri, api_key, secret_key, customer_id))

    return r.json()

def get_ad_list(base_url, api_key, secret_key, customer_id, ids):
    uri = '/ncc/ads'
    method = 'GET'
    r = requests.get(base_url + uri, params={'nccAdgroupId' :ids, 'recordSize' :1000},
                     headers=get_header(method, uri, api_key, secret_key, customer_id))

    return r.json()

def get_adkeyword_list(base_url, api_key, secret_key, customer_id, ids):
    uri = '/ncc/keywords'
    method = 'GET'
    r = requests.get(base_url + uri, params={'nccAdgroupId' :ids, 'recordSize' :1000},
                     headers=get_header(method, uri, api_key, secret_key, customer_id))

    return r.json()

def get_adkeyword_stats_list(base_url, api_key, secret_key, customer_id, ids):
    uri = '/stats'
    method = 'GET'
    r = requests.get(base_url + uri, params={'ids' :ids, 'fields': '["clkCnt","impCnt","pcNxAvgRnk", "mblNxAvgRnk"]', 'datePreset' : 'last7days'},
                     headers=get_header(method, uri, api_key, secret_key, customer_id))

    return r.json()

def get_adgroup_targets_list(base_url, api_key, secret_key, customer_id, ids):
    uri = '/ncc/targets'
    method = 'GET'
    r = requests.get(base_url + uri, params={'ownerId': ids, 'types' :['PC_MOBILE_TARGET']},
                     headers=get_header(method, uri, api_key, secret_key, customer_id))

    return r.json()




###############################################################################################
#
#
# -- Make Method (Adbid Data) 선언
#
#
###############################################################################################
def make_campaign_list():
    raw_cmp = get_campaign_list(BASE_URL, API_KEY, SECRET_KEY, CUSTOMER_ID)
    cmp_data = []

    for eachCmpData in raw_cmp:
        tmp = {}
        tmp['nccCampaignId'] = eachCmpData['nccCampaignId']
        tmp['cmpName'] = eachCmpData['name']
        tmp['campaignTp'] = eachCmpData['campaignTp']
        cmp_data.append(tmp)

    return cmp_data

def make_adgroup_list():
    raw_grp = get_adgroup_list(BASE_URL, API_KEY, SECRET_KEY, CUSTOMER_ID)
    grp_data = []

    for eachGrpData in raw_grp:
        tmp = {}
        tmp['nccAdgroupId'] = eachGrpData['nccAdgroupId']
        tmp['grpName'] = eachGrpData['name']
        tmp['nccCampaignId'] = eachGrpData['nccCampaignId']
        grp_data.append(tmp)

    return grp_data

def merge_cmp_and_grp_list(get_cmp_data, get_grp_data):
    for eachGrpData in get_grp_data:
        for eachCmpData in get_cmp_data:
            if (eachCmpData['nccCampaignId'] == eachGrpData['nccCampaignId']):
                eachGrpData['cmpName'] = eachCmpData['cmpName']
                eachGrpData['campaignTp'] = eachCmpData['campaignTp']

    return get_grp_data

def make_adkeyword_list(grp_ids):
    #################################
    # Keyword Data 호출 부 (keywords / Ads 따로 호출)
    #################################
    kwd_data = []
    ad_data = []

    for eachGrpData in grp_ids:
        if (eachGrpData['campaignTp'] == 'WEB_SITE'):
            raw_kwd = get_adkeyword_list(BASE_URL, API_KEY, SECRET_KEY, CUSTOMER_ID, eachGrpData['nccAdgroupId'])

            for eachKwdData in raw_kwd:
                tmp = {}
                tmp['id'] = eachKwdData['nccKeywordId']
                tmp['nccAdgroupId'] = eachKwdData['nccAdgroupId']
                tmp['type'] = 'easter_egg'
                tmp['kwdName'] = eachKwdData['keyword']
                tmp['bidAmt'] = eachKwdData['bidAmt']
                tmp['status'] = eachKwdData['status']

                kwd_data.append(tmp)
        elif (eachGrpData['campaignTp'] == 'SHOPPING'):
            raw_ad = get_ad_list(BASE_URL, API_KEY, SECRET_KEY, CUSTOMER_ID, eachGrpData['nccAdgroupId'])

            for eachAdData in raw_ad:
                tmp = {}
                tmp['id'] = eachAdData['nccAdId']
                tmp['nccAdgroupId'] = eachAdData['nccAdgroupId']
                tmp['type'] = eachAdData['type']
                tmp['kwdName'] = eachAdData['referenceData']['productTitle']
                tmp['bidAmt'] = eachAdData['adAttr']['bidAmt']
                tmp['status'] = eachAdData['status']

                ad_data.append(tmp)
        elif (eachGrpData['campaignTp'] == 'POWER_CONTENTS'):
            raw_kwd = get_adkeyword_list(BASE_URL, API_KEY, SECRET_KEY, CUSTOMER_ID, eachGrpData['nccAdgroupId'])

            for eachKwdData in raw_kwd:
                tmp = {}
                tmp['id'] = eachKwdData['nccKeywordId']
                tmp['nccAdgroupId'] = eachKwdData['nccAdgroupId']
                tmp['type'] = 'easter_egg'
                tmp['kwdName'] = eachKwdData['keyword']
                tmp['bidAmt'] = eachKwdData['bidAmt']
                tmp['status'] = eachKwdData['status']

                kwd_data.append(tmp)
        else:
            easter_egg = '이외 캠페인은 지원하지 않음.'

    #################################
    # Keywords / Ads Ids 전처리 부
    #################################
    total_kwd_ids = []

    for each_id in kwd_data:
        kwd_ids = each_id['id']
        total_kwd_ids.append(kwd_ids)

    total_ad_ids = []

    for each_id in ad_data:
        kwd_ids = each_id['id']
        total_ad_ids.append(kwd_ids)

    total_kwd_ids = [total_kwd_ids[t:(t + 200)] for t in range(0, len(total_kwd_ids), 200)]
    total_ad_ids = [total_ad_ids[t:(t + 200)] for t in range(0, len(total_ad_ids), 200)]


    #################################
    # Keywords Stats Data 호출 부
    #################################

    kwd_stats_data = []

    for each_200 in total_kwd_ids:
        raw_stats = get_adkeyword_stats_list(BASE_URL, API_KEY, SECRET_KEY, CUSTOMER_ID, each_200)
        raw_stats = raw_stats['data']

        if (len(raw_stats) == 0):
            for kwd_id in each_200:
                kwd_stats_init = {'id': kwd_id, 'mblNxAvgRnk': 0, 'impCnt': 0, 'pcNxAvgRnk': 0, 'clkCnt': 0}
                kwd_stats_data.append(kwd_stats_init)

        for tmp in raw_stats:
            kwd_stats_data.append(tmp)


    #################################
    # Ads Stats Data 호출 부
    #################################

    ad_stats_data = []

    for each_200 in total_ad_ids:
        raw_stats = get_adkeyword_stats_list(BASE_URL, API_KEY, SECRET_KEY, CUSTOMER_ID, each_200)
        raw_stats = raw_stats['data']

        if (len(raw_stats) == 0):
            for ad_id in each_200:
                ad_stats_init = {'id': ad_id, 'mblNxAvgRnk': 0, 'impCnt': 0, 'pcNxAvgRnk': 0, 'clkCnt': 0}
                ad_stats_data.append(ad_stats_init)

        for tmp in raw_stats:
            ad_stats_data.append(tmp)



    # Merge Keywords & Stats
    kwd_tmp = pd.DataFrame(kwd_data)
    ad_tmp = pd.DataFrame(ad_data)
    kwd_sts_tmp = pd.DataFrame(kwd_stats_data)
    ad_sts_tmp = pd.DataFrame(ad_stats_data)

    concat_data = pd.concat([kwd_tmp, ad_tmp], axis=0)
    concat_stats = pd.concat([kwd_sts_tmp, ad_sts_tmp], axis=0)

    merge_data = pd.merge(concat_data, concat_stats, how='outer')
    merge_data = merge_data.to_dict('records')

    return merge_data

def make_adgroup_targets_list(get_grp_data):
    target_data = []

    for eachGrpData in get_grp_data:
        raw_target = get_adgroup_targets_list(BASE_URL, API_KEY, SECRET_KEY, CUSTOMER_ID, eachGrpData['nccAdgroupId'])

        tmp = {}
        tmp['nccAdgroupId'] = raw_target[0]['ownerId']
        tmp['pc'] = raw_target[0]['target']['pc']
        tmp['mobile'] = raw_target[0]['target']['mobile']
        target_data.append(tmp)

    return target_data









###############################################################################################
#
#
# -- Update Method (Adbid Data) 선언
#
#
###############################################################################################
def update_keyword_bid(kwd_input):
    total_kwd_bid = {}
    total_ad_bid = {}

    for eachInput in kwd_input:
        if (eachInput['campaignTp'] != 'SHOPPING') :

            #################################################
            # -- Position Bid 호출 부
            #################################################
            uri_post = '/estimate/average-position-bid/id'
            method_post = 'POST'

            data = {"device": eachInput['device'], "items": [{"key": eachInput['id'], "position": eachInput['position']}]}
            pos_fields_data = jsonpickle.encode(data, unpicklable=False)

            r_post = requests.post(BASE_URL + uri_post, data=pos_fields_data, headers=get_header(method_post, uri_post, API_KEY, SECRET_KEY, CUSTOMER_ID))
            posBid = r_post.json()['estimate'][0]['bid']


            #################################################
            # -- Keyword Bid Update 호출 부
            #################################################
            uri = '/ncc/keywords/' + eachInput['id']
            method = 'PUT'

            data = {"bidAmt": min((int(posBid) + int(eachInput['addBidAmt'])), int(eachInput['maxBidAmt'])), "nccAdgroupId": eachInput['nccAdgroupId'], "nccKeywordId": eachInput['id'], "useGroupBidAmt": False}
            bid_fields_data = jsonpickle.encode(data, unpicklable=False)

            r = requests.put(BASE_URL + uri + '?fields=bidAmt', data=bid_fields_data, headers=get_header(method, uri, API_KEY, SECRET_KEY, CUSTOMER_ID))

            total_kwd_bid[eachInput['id']] = r.json()
        else :
            #################################################
            # -- Position Bid 호출 부
            #################################################
            uri_post = '/npla-estimate/average-position-bid/id'
            method_post = 'POST'

            data = {"device": eachInput['device'], "items": [{"key": eachInput['id'], "position": eachInput['position']}]}
            pos_fields_data = jsonpickle.encode(data, unpicklable=False)
            r_post = requests.post(BASE_URL + uri_post, data=pos_fields_data, headers=get_header(method_post, uri_post, API_KEY, SECRET_KEY, CUSTOMER_ID))

            pos_bid = r_post.json()['estimate'][0]['bid']

            #################################################
            # -- AdGroup Bid Update 호출 부
            #################################################
            uri = '/ncc/ads/' + eachInput['id']
            method = 'PUT'

            data = {"type": eachInput['type'], "adAttr": {"bidAmt": min((int(pos_bid) + int(eachInput['addBidAmt'])), int(eachInput['maxBidAmt'])), "useGroupBidAmt": False}, "nccAdId": eachInput['id']}
            bid_fields_data = jsonpickle.encode(data, unpicklable=False)

            r = requests.put(BASE_URL + uri + '?fields=adAttr', data=bid_fields_data,
                             headers=get_header(method, uri, API_KEY, SECRET_KEY, CUSTOMER_ID))

            total_ad_bid[eachInput['id']] = r.json()

    #################################
    # Kwd & Ad Bid Update Result Merge
    #################################
    total_kwd_bid.update(total_ad_bid)

    return total_kwd_bid

def update_keyword_stats(kwd_input):
    #################################
    # -- input_data에서 kwd / ad 200 분류
    #################################
    total_kwd_ids = []
    total_ad_ids = []

    for each_id in kwd_input:
        if (each_id['campaignTp'] != 'SHOPPING'):
            total_kwd_ids.append(each_id['id'])
        else:
            total_ad_ids.append(each_id['id'])

    total_kwd_ids = [total_kwd_ids[t:(t + 200)] for t in range(0, len(total_kwd_ids), 200)]
    total_ad_ids = [total_ad_ids[t:(t + 200)] for t in range(0, len(total_ad_ids), 200)]

    #################################
    # Keywords Stats Data 호출 부
    #################################
    total_kwd_stats = {}

    for each_200 in total_kwd_ids:
        raw_stats = get_adkeyword_stats_list(BASE_URL, API_KEY, SECRET_KEY, CUSTOMER_ID, each_200)
        raw_stats = raw_stats['data']

        if (len(raw_stats) == 0):
            for kwd_id in each_200:
                total_kwd_stats[kwd_id] = {'id': kwd_id, 'mblNxAvgRnk': 0, 'impCnt': 0, 'pcNxAvgRnk': 0, 'clkCnt': 0}

        for tmp in raw_stats:
            total_kwd_stats[tmp['id']] = tmp

    #################################
    # Ads Stats Data 호출 부
    #################################
    total_ad_stats = {}

    for each_200 in total_ad_ids:
        raw_stats = get_adkeyword_stats_list(BASE_URL, API_KEY, SECRET_KEY, CUSTOMER_ID, each_200)
        raw_stats = raw_stats['data']

        if (len(raw_stats) == 0):
            for ad_id in each_200:
                total_ad_stats[ad_id] = {'id': ad_id, 'mblNxAvgRnk': 0, 'impCnt': 0, 'pcNxAvgRnk': 0, 'clkCnt': 0}

        for tmp in raw_stats:
            total_ad_stats[tmp['id']] = tmp

    #################################
    # Kwd & Ad Stats Merge
    #################################
    total_kwd_stats.update(total_ad_stats)

    return total_kwd_stats










###############################################################################################
#
#
# -- Python 전체 로직 실행 부분
#
#
###############################################################################################

if (len(kwd_data) == 0):
    ################################################################
    # --Get Total Data 호출 부
    ################################################################

    # --Get Campaign / AdGroup Data
    get_cmp_data = make_campaign_list()
    get_grp_data = make_adgroup_list()

    # --Make Kwd / Stats / Targets Data (DataFrame)
    grp = merge_cmp_and_grp_list(get_cmp_data, get_grp_data)
    df_grp = pd.DataFrame(grp)
    kwd = make_adkeyword_list(grp)
    df_kwd = pd.DataFrame(kwd)
    trg = make_adgroup_targets_list(grp)
    df_trg = pd.DataFrame(trg)

    # --Merge Ad Total Data
    get_total_list_df = pd.merge(df_grp, df_kwd)
    get_total_list_df = pd.merge(get_total_list_df, df_trg)
    get_total_list_df = get_total_list_df.fillna('0')

    # --Make Ad Total Data (DataFrame to JSON)
    get_total_list = get_total_list_df.to_dict('record')

    # --Make Ad Total Data (JSON to String)
    getFuncReturnData = ''

    for eachJSON in get_total_list:
        getFuncReturnData += 'nccAdgroupId=' + eachJSON['nccAdgroupId'] + '/./'
        getFuncReturnData += 'grpName=' + eachJSON['grpName'] + '/./'
        getFuncReturnData += 'nccCampaignId=' + eachJSON['nccCampaignId'] + '/./'
        getFuncReturnData += 'cmpName=' + eachJSON['cmpName'] + '/./'
        getFuncReturnData += 'campaignTp=' + eachJSON['campaignTp'] + '/./'
        getFuncReturnData += 'id=' + eachJSON['id'] + '/./'
        getFuncReturnData += 'kwdName=' + eachJSON['kwdName'] + '/./'
        getFuncReturnData += 'type=' + eachJSON['type'] + '/./'
        getFuncReturnData += 'status=' + eachJSON['status'] + '/./'
        getFuncReturnData += 'bidAmt=' + str(eachJSON['bidAmt']) + '/./'
        getFuncReturnData += 'pcNxAvgRnk=' + str(eachJSON['pcNxAvgRnk']) + '/./'
        getFuncReturnData += 'mblNxAvgRnk=' + str(eachJSON['mblNxAvgRnk']) + '/./'
        getFuncReturnData += 'clkCnt=' + str(int(eachJSON['clkCnt'])) + '/./'
        getFuncReturnData += 'impCnt=' + str(int(eachJSON['impCnt'])) + '/./'
        getFuncReturnData += 'pc=' + str(eachJSON['pc']) + '/./'
        getFuncReturnData += 'mobile=' + str(eachJSON['mobile']) + '^.^'

    print(getFuncReturnData)

else:
    ################################################################
    # --Update Total Data 호출 부
    ################################################################

    # --Kwd Data Parser 호출 부
    kwd_data = kwd_data.split('\r\n')
    kwd_data = kwd_data[1:(len(kwd_data)-1)]
    kwd_data = [kwd_data[t:(t + 12)] for t in range(0, len(kwd_data), 12)]

    kwd_data_json = []
    for t in kwd_data:
        tmp = {}
        tmp['nccAdgroupId'] = t[1].replace(',', '').split(': ')[1]
        tmp['id'] = t[2].replace(',', '').split(': ')[1]
        tmp['device'] = 'mobile'
        if t[6].replace(',', '').split(': ')[1] != '0':
            tmp['position'] = t[6].replace(',', '').split(': ')[1]
        else:
            tmp['position'] = 3
        if t[7].replace(',', '').split(': ')[1] != 'null':
            tmp['maxBidAmt'] = t[7].replace(',', '').split(': ')[1]
        else:
            tmp['maxBidAmt'] = 70
        if t[9].replace(',', '').split(': ')[1] != '"':
            tmp['addBidAmt'] = t[9].replace(',', '').split(': ')[1]
        else:
            tmp['addBidAmt'] = 70
        tmp['campaignTp'] = t[8].replace(',', '').split(': ')[1]
        tmp['type'] = t[10].replace(',', '').split(': ')[1]
        kwd_data_json.append(tmp)


    # --Update Bid & Stats 호출 부
    update_bid_list = update_keyword_bid(kwd_data_json)
    update_stats_list = update_keyword_stats(kwd_data_json)

    # --Update / Get Stats Column 추출출
    result_update = []
    for each_row in update_bid_list.values():
        tmp = {}
        if 'adAttr' not in each_row.keys():
            tmp['id'] = each_row['nccKeywordId']
            tmp['bidAmt'] = each_row['bidAmt']
            tmp['status'] = each_row['status']
        else:
            tmp['id'] = each_row['nccAdId']
            tmp['bidAmt'] = each_row['adAttr']['bidAmt']
            tmp['status'] = each_row['status']
        result_update.append(tmp)

    result_stats_get = []
    for each_row in update_stats_list.values():
        tmp = {}
        tmp['id'] = each_row['id']
        tmp['pcNxAvgRnk'] = each_row['pcNxAvgRnk']
        tmp['mblNxAvgRnk'] = each_row['mblNxAvgRnk']
        tmp['clkCnt'] = each_row['clkCnt']
        tmp['impCnt'] = each_row['impCnt']
        result_stats_get.append(tmp)

    # --Update Result Data Making (DataFrame)
    result_update_df = pd.DataFrame(result_update)
    result_stats_get_df = pd.DataFrame(result_stats_get)

    # -- Merge and Update Result Data Making (DataFrame to JSON)
    # print(result_update_df)
    # print(result_stats_get_df)
    update_total_list_df = pd.merge(result_update_df, result_stats_get_df, how='left', on='id')
    update_total_list_df = update_total_list_df.fillna('0')
    update_total_list = update_total_list_df.to_dict('record')

    # --Update Result Data Making (JSON to String)
    getFuncReturnData = ''

    for eachJSON in update_total_list:
        getFuncReturnData += 'id=' + eachJSON['id'] + '/./'
        getFuncReturnData += 'bidAmt=' + str(eachJSON['bidAmt']) + '/./'
        getFuncReturnData += 'status=' + eachJSON['status'] + '/./'
        getFuncReturnData += 'pcNxAvgRnk=' + str(eachJSON['pcNxAvgRnk']) + '/./'
        getFuncReturnData += 'mblNxAvgRnk=' + str(eachJSON['mblNxAvgRnk']) + '/./'
        getFuncReturnData += 'clkCnt=' + str(int(eachJSON['clkCnt'])) + '/./'
        getFuncReturnData += 'impCnt=' + str(int(eachJSON['impCnt'])) + '^.^'

    print(getFuncReturnData)





