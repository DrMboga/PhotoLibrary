import * as React from 'react';
import { MediaInfo } from '../model/mediaInfo';
import { Box, Card, CardContent, CardMedia, Typography } from '@mui/material';
import PlayCircleIcon from '@mui/icons-material/PlayCircle';
import { dateFromUnixTime, secondsToTimeFormat } from '../helpers/date-helper';

// TODO: Add thumbnail width and height to the media data model
const largestSize = 224;
const smallestSize = 168;

type Props = {
  media: MediaInfo;
};
export const MediaCard = ({ media }: Props) => {
  const mediaDate = dateFromUnixTime(media.dateTimeOriginal);
  const country = media.country ?? '';
  const city = media.locality ?? media.region ?? '';

  const address = city && country ? `${city}, ${country}` : '';
  const videoDuration: string = media.videoDurationSec
    ? secondsToTimeFormat(media.videoDurationSec)
    : '';
  // #004d40

  return (
    <Card key={`card-${media.id}`}>
      <Box sx={{ position: 'relative' }}>
        <CardMedia
          key={`card-media-${media.id}`}
          component="img"
          width={media.width > media.height ? largestSize : smallestSize}
          height={media.width < media.height ? largestSize : smallestSize}
          image={media.thumbnailUrl}
        ></CardMedia>
        {media.mediaType === 'video' && (
          <Typography
            id={`typography-0-${media.id}`}
            sx={{
              fontSize: '10px',
              display: 'flex',
              alignItems: 'center',
              position: 'absolute',
              top: '10px',
              left: '10px',
              gap: '5px',
              color: 'white',
            }}
          >
            <PlayCircleIcon />
            {videoDuration}
          </Typography>
        )}
      </Box>
      <CardContent
        id={`card-content-1-${media.id}`}
        sx={{
          padding: '2px',
          '&:last-child': {
            paddingBottom: 0,
          },
        }}
      >
        <Typography id={`typography-1-${media.id}`} variant="body2" align="right">
          {mediaDate.toLocaleString('ru-RU')}
        </Typography>
      </CardContent>
    </Card>
  );
};
