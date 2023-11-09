import * as React from 'react';
import { MediaInfo } from '../model/mediaInfo';
import { Card, CardActions, CardContent, CardMedia, IconButton, Typography } from '@mui/material';
import FavoriteIcon from '@mui/icons-material/Favorite';
import ShareIcon from '@mui/icons-material/Share';
import PlaceIcon from '@mui/icons-material/Place';
import PlayCircleIcon from '@mui/icons-material/PlayCircle';
import { dateFromUnixTime } from '../helpers/date-helper';

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
  // #004d40

  return (
    <Card key={`card-${media.id}`}>
      <CardMedia
        key={`card-media-${media.id}`}
        component="img"
        width={media.width > media.height ? largestSize : smallestSize}
        height={media.width < media.height ? largestSize : smallestSize}
        image={media.thumbnailUrl}
      />
      <CardContent
        id={`card-content-1-${media.id}`}
        sx={{ paddingTop: '1px', paddingBottom: '0px' }}
      >
        {media.mediaType === 'video' && (
          <Typography
            id={`typography-0-${media.id}`}
            sx={{ fontSize: '10px', display: 'flex', alignItems: 'center' }}
            color="text.secondary"
            align="right"
          >
            <PlayCircleIcon />
            {media.videoDurationSec}
          </Typography>
        )}
        <Typography
          id={`typography-2-${media.id}`}
          sx={{ fontSize: '10px' }}
          color="text.secondary"
          align="right"
        >
          {media.fullPath}
        </Typography>
        <Typography id={`typography-1-${media.id}`} variant="body2" align="right">
          {mediaDate.toLocaleString('ru-RU')}
        </Typography>
        {address && (
          <Typography
            id={`typography-1-${media.id}`}
            sx={{ fontSize: '12px', display: 'flex', alignItems: 'center' }}
            color="text.secondary"
            align="right"
          >
            <PlaceIcon />
            {address}
          </Typography>
        )}
      </CardContent>

      <CardActions disableSpacing sx={{ paddingTop: '1px', paddingBottom: '1px' }}>
        <IconButton aria-label="add to favorites" size="small">
          <FavoriteIcon />
        </IconButton>
        <IconButton aria-label="share" size="small">
          <ShareIcon />
        </IconButton>
      </CardActions>
    </Card>
  );
};
