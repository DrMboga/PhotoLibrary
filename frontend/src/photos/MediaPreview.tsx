import * as React from 'react';
import {
  Card,
  CardContent,
  CardHeader,
  CardMedia,
  CardActions,
  Typography,
  IconButton,
} from '@mui/material';
import { dateFromUnixTime } from '../helpers/date-helper';
import FavoriteIcon from '@mui/icons-material/Favorite';
import ShareIcon from '@mui/icons-material/Share';
import { MediaInfo } from '../model/media-info';

// TODO: Add thumbnail width and height to the media data model
const largestSize = 224;
const smallestSize = 168;

type Props = {
  media: MediaInfo;
};
export const MediaPreview = ({ media }: Props) => {
  const mediaDate = dateFromUnixTime(media.dateTimeOriginal);
  const country = media.country ?? '';
  const city = media.locality ?? media.region ?? '';
  const venue = media.venue ?? '';

  const address = city && country ? `${city}, ${country}${venue ? `, ${venue}` : ''}` : '';
  return (
    <Card key={`media-preview-card${media.id}`}>
      <CardHeader subheader={media.fullPath} />
      <CardMedia
        key={`card-media-${media.id}`}
        component="img"
        width={media.width > media.height ? largestSize * 3 : smallestSize * 3}
        height={media.width < media.height ? largestSize * 3 : smallestSize * 3}
        image={media.thumbnailUrl}
      ></CardMedia>
      <CardContent
        id={`card-content-1-${media.id}`}
        sx={{ paddingTop: '1px', paddingBottom: '0px' }}
      >
        <Typography id={`typography-1-${media.id}`} variant="body2">
          {mediaDate.toLocaleString('ru-RU')}
        </Typography>
        {address && (
          <Typography
            id={`typography-1-${media.id}`}
            sx={{
              fontSize: '12px',
              display: 'flex',
              alignItems: 'center',
            }}
            color="text.secondary"
          >
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
