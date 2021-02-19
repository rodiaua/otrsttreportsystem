import * as moment from 'moment';
import * as momentTimezone from 'moment-timezone';

export class EpochConverter{
    public static toHumanReadableString( utcTime?: number): string {
        if(utcTime != null)
          return momentTimezone(momentTimezone(utcTime * 1000).utcOffset(0)).format('DD.MM.YYYY-HH:mm:ss');
        else
          return '';
      }
}