import * as React from 'react';
import { Box, Card, CardContent, CardMedia, Typography } from '@mui/material';
import PlayCircleIcon from '@mui/icons-material/PlayCircle';
import { dateFromUnixTime, secondsToTimeFormat } from '../helpers/date-helper';
import { MediaInfo, MediaType } from '../model/media-info';

// TODO: Add thumbnail width and height to the media data model
const largestSize = 224;
const smallestSize = 168;

type Props = {
  media: MediaInfo;
  onClick: (mediaId: string) => void;
};
export const MediaCard = ({ media, onClick }: Props) => {
  const mediaDate = dateFromUnixTime(media.dateTimeOriginal);
  const videoDuration: string = media.videoDurationSec
    ? secondsToTimeFormat(media.videoDurationSec)
    : '';
  // #004d40

  return (
    <Card key={`card-${media.id}`} onClick={() => onClick(media.id)}>
      <Box sx={{ position: 'relative' }}>
        <CardMedia
          key={`card-media-${media.id}`}
          component="img"
          width={media.width > media.height ? largestSize : smallestSize}
          height={media.width < media.height ? largestSize : smallestSize}
          image={media.thumbnailUrl}
        ></CardMedia>
        {media.mediaType === MediaType.VIDEO && (
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
