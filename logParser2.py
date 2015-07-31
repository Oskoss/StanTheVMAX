import os
import re
import tweet
import time

LATEST_ENTRY = 0 #line number

def test():
	
	global LATEST_ENTRY
	while (1):
		file_obj = open('C:\Program Files\EMC\SYMAPI\log\my_event_file.log0')
		text = file_obj.read()
		file_obj.close()
		log_text = text.split()
		for index in xrange(LATEST_ENTRY+1,len(log_text)):
			if index + 3 >= len(log_text): break
			'''if log_text[index] == '\'SET\'' and log_text[index + 3].startswith('SLO_NAME'):
				slo = log_text[index+3][9:]
				if slo == 'Diamond':
					tweet.tweet_string("Diamonds are a girls best friend! %s %s SLO headed to Diamond Power! #LookAtThatRock!" % (log_text[index-2][:-8], log_text[index-1][:-14]))
					print "tweeted Diamond"
					time.sleep(5)
				if slo == 'Gold':
					tweet.tweet_string("I've struck gold. %s %s SLO upgraded to Gold! #MovinOnUp" % (log_text[index-2][:-8], log_text[index-1][:-14]))
					print "tweeted Gold"
					time.sleep(5)
				if slo == 'SLONONE':
					tweet.tweet_string("I got it covered!! Switching %s %s SLO to Optimized #Coexist" % (log_text[index-2][:-8], log_text[index-1][:-14]))
					print "tweeted Optimized"
					time.sleep(5) 
			if log_text[index] == '\'DELETE\'' and log_text[index - 1] == 'group)' and log_text[index - 2].endswith('(Storage'):
				slo = log_text[index - 2][:-8]
				tweet.tweet_string("Hey, SLO %s hasta la vista baby!! #SpringCleaning" % slo.upper())
				print "tweeted Delete"
				time.sleep(5)
				'''		
			# Create Operations
			#Storage Group creation detected
			if log_text[index] == '\'CREATE\'' and log_text[index - 1] == 'group)' and log_text[index - 2].endswith('(Storage'):
				sg_name = log_text[index - 2][:-8]
				symid = log_text[index + 2][5:]
				symlen = len(symid)
				symid = symid[0:symlen - 1]
				str(symid)
				# the line below should get should yield [date=2015-03-20T00:20:27]
				time_stamp = log_text[index - 15] # 20:27
				date_stamp = time_stamp[6:16] # 2015-03-20
				time_stamp = time_stamp[17:25]					
				 #tweet.tweet_string("the symm %s has created the storage group %s [%s %s]" % (symid, sg_name, date_stamp, time_stamp ))
				print ("the symm %s has created the storage group %s [%s %s]" % (symid, sg_name, date_stamp, time_stamp ))
				 
			elif log_text[index] == 'Created' and log_text[index + 1] == 'Masking' and log_text[index + 2] == 'View':
				# we've got a new masking view
				masking_view_name = log_text[index + 3]
				date_stamp = log_text[index - 12]
				time_stamp = date_stamp[20:25]
				date_stamp = date_stamp[6:16]
				#tweet.tweet_string("We've got a new Masking View named" + masking_view_name + "created on " +
				#				   date_stamp + " at " + time_stamp  )
								   
			# pretty sure that has to be a newly created device.
			elif log_text[index] == 'create' and log_text[index + 1] == 'dev' and log_text[index+ 2] == 'count':
				# unfortunately nothing really useful is logged by default
				#tweet.tweet_string("on symm %s a new device has been created! on %s at %s") % (symid, date_stamp, time_stamp)
				#Devices start then complete on different lines..look for the start cycle for a bit and look for success.
				


		time.sleep(5)

		print LATEST_ENTRY
		LATEST_ENTRY = len(log_text)
		time.sleep(5)
test()

			