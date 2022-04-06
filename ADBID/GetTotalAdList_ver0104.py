import time
import random
import requests
import hashlib
import hmac
import base64
from datetime import datetime
import sys
import pandas as pd
import numpy as np

start = time.time()

BASE_URL = sys.argv[1]
API_KEY = sys.argv[2]
SECRET_KEY = sys.argv[3]
CUSTOMER_ID = sys.argv[4]
AUTO_BID_JSON = sys.argv[5]

# BASE_URL = 'https://api.naver.com'
# API_KEY = '01000000001319784e11d961570b647e633339027356f1e4b0773f54aaceb0f28abff8b77f'
# SECRET_KEY = 'AQAAAAATGXhOEdlhVwtkfmMzOQJzLbzdxBBcNtW1JYQRyZQn9Q=='
# CUSTOMER_ID = '770790'

# #####################################################
# # --RestAPI Message Func (Header 함수 Merge)
# #####################################################
# def get_header(method, uri, api_key, secret_key, customer_id):
#     timestamp = str(round(time.time() * 1000))
#     message = "{}.{}.{}".format(timestamp, method, uri)
#     hash = hmac.new(bytes(secret_key, "utf-8"), bytes(message, "utf-8"), hashlib.sha256)
#     hash.hexdigest()
#     signature = base64.b64encode(hash.digest())
#     return {'Content-Type': 'application/json; charset=UTF-8', 'X-Timestamp': timestamp, 'X-API-KEY': API_KEY,
#             'X-Customer': str(CUSTOMER_ID), 'X-Signature': signature}
#
#
#
#
# #####################################################
# # --Naver API Method
# #####################################################
# def getCampaignList(BASE_URL, API_KEY, SECRET_KEY, CUSTOMER_ID):
#     uri = '/ncc/campaigns'
#     method = 'GET'
#     r = requests.get(BASE_URL + uri, headers=get_header(method, uri, API_KEY, SECRET_KEY, CUSTOMER_ID))
#
#     return r.json()
#
#
# def getAdGroupList(BASE_URL, API_KEY, SECRET_KEY, CUSTOMER_ID):
#     uri = '/ncc/adgroups'
#     method = 'GET'
#     r = requests.get(BASE_URL + uri, headers=get_header(method, uri, API_KEY, SECRET_KEY, CUSTOMER_ID))
#
#     return r.json()
#
#
# def getAdList(BASE_URL, API_KEY, SECRET_KEY, CUSTOMER_ID, ids):
#     uri = '/ncc/ads'
#     method = 'GET'
#     r = requests.get(BASE_URL + uri, params={'nccAdgroupId': ids, 'recordSize': 1000},
#                      headers=get_header(method, uri, API_KEY, SECRET_KEY, CUSTOMER_ID))
#
#     return r.json()
#
#
# def getAdKeywordList(BASE_URL, API_KEY, SECRET_KEY, CUSTOMER_ID, ids):
#     uri = '/ncc/keywords'
#     method = 'GET'
#     r = requests.get(BASE_URL + uri, params={'nccAdgroupId': ids, 'recordSize': 1000},
#                      headers=get_header(method, uri, API_KEY, SECRET_KEY, CUSTOMER_ID))
#
#     return r.json()
#
#
# def getAdKeywordsStatsList(BASE_URL, API_KEY, SECRET_KEY, CUSTOMER_ID, ids):
#     uri = '/stats'
#     method = 'GET'
#     r = requests.get(BASE_URL + uri, params={'ids': ids, 'fields': '["clkCnt","impCnt","pcNxAvgRnk", "mblNxAvgRnk"]',
#                                              'datePreset': 'last7days'},
#                      headers=get_header(method, uri, API_KEY, SECRET_KEY, CUSTOMER_ID))
#
#     return r.json()
#
#
# def getAdGroupTargetsList(BASE_URL, API_KEY, SECRET_KEY, CUSTOMER_ID, ids):
#     uri = '/ncc/targets'
#     method = 'GET'
#     r = requests.get(BASE_URL + uri, params={'ownerId': ids, 'types': ['PC_MOBILE_TARGET']},
#                      headers=get_header(method, uri, API_KEY, SECRET_KEY, CUSTOMER_ID))
#
#     return r.json()
#
#
#
#
# #####################################################
# # --Naver API Method
# #####################################################
# def makeCampaignList():
#     raw_cmp = getCampaignList(BASE_URL, API_KEY, SECRET_KEY, CUSTOMER_ID)
#     cmp_data = []
#
#     for eachCmpData in raw_cmp:
#         tmp = {}
#         tmp['nccCampaignId'] = eachCmpData['nccCampaignId']
#         tmp['cmpName'] = eachCmpData['name']
#         tmp['campaignTp'] = eachCmpData['campaignTp']
#         cmp_data.append(tmp)
#
#     return cmp_data
#
#
# def makeAdGroupList():
#     raw_grp = getAdGroupList(BASE_URL, API_KEY, SECRET_KEY, CUSTOMER_ID)
#     grp_data = []
#
#     for eachGrpData in raw_grp:
#         tmp = {}
#         tmp['nccAdgroupId'] = eachGrpData['nccAdgroupId']
#         tmp['grpName'] = eachGrpData['name']
#         tmp['nccCampaignId'] = eachGrpData['nccCampaignId']
#         grp_data.append(tmp)
#
#     return grp_data
#
#
# def mergeCmpAndGrpList():
#     get_grp_data = makeAdGroupList()
#     get_cmp_data = makeCampaignList()
#     for eachGrpData in get_grp_data:
#         for eachCmpData in get_cmp_data:
#             if (eachCmpData['nccCampaignId'] == eachGrpData['nccCampaignId']):
#                 eachGrpData['cmpName'] = eachCmpData['cmpName']
#                 eachGrpData['campaignTp'] = eachCmpData['campaignTp']
#
#     return get_grp_data
#
# ###########################################
# # --stats ids 100개씩 넘기기
# ###########################################
# def makeAdKeywordList(grp_ids):
#     #################################
#     # Keyword Data 호출 부 (keywords / Ads 따로 호출)
#     #################################
#     kwd_data = []
#     ad_data = []
#     for eachGrpData in grp_ids:
#         if (eachGrpData['campaignTp'] == 'WEB_SITE'):
#             raw_kwd = getAdKeywordList(BASE_URL, API_KEY, SECRET_KEY, CUSTOMER_ID, eachGrpData['nccAdgroupId'])
#
#             for eachKwdData in raw_kwd:
#                 tmp = {}
#                 tmp['id'] = eachKwdData['nccKeywordId']
#                 tmp['nccAdgroupId'] = eachKwdData['nccAdgroupId']
#                 # tmp['nccCampaignId'] = eachKwdData['nccCampaignId']
#                 tmp['kwdName'] = eachKwdData['keyword']
#                 tmp['bidAmt'] = eachKwdData['bidAmt']
#
#                 kwd_data.append(tmp)
#         elif (eachGrpData['campaignTp'] == 'SHOPPING'):
#             raw_ad = getAdList(BASE_URL, API_KEY, SECRET_KEY, CUSTOMER_ID, eachGrpData['nccAdgroupId'])
#
#             for eachAdData in raw_ad:
#                 tmp = {}
#                 tmp['id'] = eachAdData['nccAdId']
#                 tmp['nccAdgroupId'] = eachAdData['nccAdgroupId']
#                 tmp['kwdName'] = eachAdData['referenceData']['productTitle']
#                 tmp['bidAmt'] = eachAdData['adAttr']['bidAmt']
#
#                 ad_data.append(tmp)
#         elif (eachGrpData['campaignTp'] == 'POWER_CONTENTS'):
#             raw_kwd = getAdKeywordList(BASE_URL, API_KEY, SECRET_KEY, CUSTOMER_ID, eachGrpData['nccAdgroupId'])
#
#             for eachKwdData in raw_kwd:
#                 tmp = {}
#                 tmp['id'] = eachKwdData['nccKeywordId']
#                 tmp['nccAdgroupId'] = eachKwdData['nccAdgroupId']
#                 # tmp['nccCampaignId'] = eachKwdData['nccCampaignId']
#                 tmp['kwdName'] = eachKwdData['keyword']
#                 tmp['bidAmt'] = eachKwdData['bidAmt']
#
#                 kwd_data.append(tmp)
#         else:
#             easter_egg = '이외 캠페인은 지원하지 않음.'
#
#     #################################
#     # Keywords Stats Data 호출 부
#     #################################
#     kwd_stats_data = []
#     kwd_count = 0
#     kwd_ids_100 = []
#
#     for each_row in kwd_data:
#         ncc_kwd = each_row['id']
#         kwd_ids_100.append(ncc_kwd)
#         kwd_count += 1
#         if (kwd_count % 100 == 0):
#             raw_stats = getAdKeywordsStatsList(BASE_URL, API_KEY, SECRET_KEY, CUSTOMER_ID, kwd_ids_100)
#             raw_stats = raw_stats['data']
#
#             for tmp in raw_stats:
#                 kwd_stats_data.append(tmp)
#             kwd_ids_100 = []
#         elif (kwd_count == len(kwd_data)):
#             raw_stats = getAdKeywordsStatsList(BASE_URL, API_KEY, SECRET_KEY, CUSTOMER_ID, kwd_ids_100)
#             raw_stats = raw_stats['data']
#
#             for tmp in raw_stats:
#                 kwd_stats_data.append(tmp)
#             kwd_ids_100 = []
#         else:
#             easter_egg = '할말없음'
#
#     #################################
#     # Ads Stats Data 호출 부
#     #################################
#     ad_stats_data = []
#     ad_count = 0
#     ad_ids_100 = []
#
#     for each_row in ad_data:
#         ncc_ad = each_row['id']
#         ad_ids_100.append(ncc_ad)
#         ad_count += 1
#         if (ad_count % 100 == 0):
#             raw_stats = getAdKeywordsStatsList(BASE_URL, API_KEY, SECRET_KEY, CUSTOMER_ID, ad_ids_100)
#             raw_stats = raw_stats['data']
#
#             for tmp in raw_stats:
#                 ad_stats_data.append(tmp)
#             ad_ids_100 = []
#         elif (ad_count == len(ad_data)):
#             raw_stats = getAdKeywordsStatsList(BASE_URL, API_KEY, SECRET_KEY, CUSTOMER_ID, ad_ids_100)
#             raw_stats = raw_stats['data']
#
#             for tmp in raw_stats:
#                 ad_stats_data.append(tmp)
#             ad_ids_100 = []
#         else:
#             easter_egg = '할말없음'
#
#     # Merge Keywords & Stats
#     kwd_tmp = pd.DataFrame(kwd_data)
#     ad_tmp = pd.DataFrame(ad_data)
#     kwd_sts_tmp = pd.DataFrame(kwd_stats_data)
#     ad_sts_tmp = pd.DataFrame(ad_stats_data)
#
#     concat_data = pd.concat([kwd_tmp, ad_tmp], axis=0)
#     concat_stats = pd.concat([kwd_sts_tmp, ad_sts_tmp], axis=0)
#
#     merge_data = pd.merge(concat_data, concat_stats, how='outer')
#     merge_data = merge_data.to_dict('records')
#
#     return merge_data
#
#
# ##########################################################
# # makeKeywordsStatsList → makeAdKeywordList 와 병함
# ##########################################################
# # def makeKeywordsStatsList():
# #    get_kwd_data = makeAdKeywordList()
# #    stats_data = []
# #
# #   for eachKwdData in get_kwd_data:
# #        raw_stats = getAdKeywordsStatsList(BASE_URL, API_KEY, SECRET_KEY, CUSTOMER_ID, eachKwdData['nccKeywordId'])
# #
# #        tmp = {}
# #        tmp['nccKeywordId'] = eachKwdData['nccKeywordId']
# #        tmp['kwdName'] = eachKwdData['kwdName']
# #        tmp['impCnt'] = raw_stats['data'][0]['impCnt']
# #        tmp['clkCnt'] = raw_stats['data'][0]['clkCnt']
# #        tmp['pcNxAvgRnk'] = raw_stats['data'][0]['pcNxAvgRnk']
# #        tmp['mblNxAvgRnk'] = raw_stats['data'][0]['mblNxAvgRnk']
# #        stats_data.append(tmp)
# #
# #    return stats_data
#
# def makeAdGroupTargetsList():
#     get_grp_data = mergeCmpAndGrpList()
#     target_data = []
#
#     for eachGrpData in get_grp_data:
#         raw_target = getAdGroupTargetsList(BASE_URL, API_KEY, SECRET_KEY, CUSTOMER_ID, eachGrpData['nccAdgroupId'])
#
#         tmp = {}
#         tmp['nccAdgroupId'] = raw_target[0]['ownerId']
#         tmp['pc'] = raw_target[0]['target']['pc']
#         tmp['mobile'] = raw_target[0]['target']['mobile']
#         target_data.append(tmp)
#
#     return target_data
#
#
#
#
# #####################################################
# # --Make Total Get Ad List (DataFrame to JSON)
# #####################################################
# grp = mergeCmpAndGrpList()
# df_grp = pd.DataFrame(grp)
# # sts = makeKeywordsStatsList()
# # df_sts = pd.DataFrame(sts)
# kwd = makeAdKeywordList(grp)
# df_kwd = pd.DataFrame(kwd)
# trg = makeAdGroupTargetsList()
# df_trg = pd.DataFrame(trg)
#
# getTotalAdList_df = pd.merge(df_grp, df_kwd)
# # getTotalAdList_df = pd.merge(getTotalAdList_df, df_sts)
# getTotalAdList_df = pd.merge(getTotalAdList_df, df_trg)
# getTotalAdList_df = getTotalAdList_df.fillna('0')
#
# getTotalAdList = getTotalAdList_df.to_dict('record')
# # print(getTotalAdList)
#
#
#
#
# #####################################################
# # --Make Total Get Ad List (JSON to String)
# #####################################################
# getFuncReturnData = ''
#
# for eachJSON in getTotalAdList:
#     getFuncReturnData += 'nccAdgroupId=' + eachJSON['nccAdgroupId'] + '/./'
#     getFuncReturnData += 'grpName=' + eachJSON['grpName'] + '/./'
#     getFuncReturnData += 'nccCampaignId=' + eachJSON['nccCampaignId'] + '/./'
#     getFuncReturnData += 'cmpName=' + eachJSON['cmpName'] + '/./'
#     getFuncReturnData += 'campaignTp=' + eachJSON['campaignTp'] + '/./'
#     getFuncReturnData += 'id=' + eachJSON['id'] + '/./'
#     getFuncReturnData += 'kwdName=' + eachJSON['kwdName'] + '/./'
#     getFuncReturnData += 'bidAmt=' + str(eachJSON['bidAmt']) + '/./'
#     getFuncReturnData += 'pcNxAvgRnk=' + str(eachJSON['pcNxAvgRnk']) + '/./'
#     getFuncReturnData += 'mblNxAvgRnk=' + str(eachJSON['mblNxAvgRnk']) + '/./'
#     getFuncReturnData += 'clkCnt=' + str(int(eachJSON['clkCnt'])) + '/./'
#     getFuncReturnData += 'impCnt=' + str(int(eachJSON['impCnt'])) + '/./'
#     getFuncReturnData += 'pc=' + str(eachJSON['pc']) + '/./'
#     getFuncReturnData += 'mobile=' + str(eachJSON['mobile']) + '^.^'
#

print(AUTO_BID_JSON)
# print(len(getTotalAdList))
# print("%f초 걸렸습니다." % (time.time() - start))




























