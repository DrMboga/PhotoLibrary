import * as React from 'react';
import {
  Card,
  CardContent,
  CardHeader,
  CardMedia,
  CardActions,
  Typography,
  IconButton,
  Box,
} from '@mui/material';
import { dateFromUnixTime } from '../helpers/date-helper';
import FavoriteIcon from '@mui/icons-material/Favorite';
import { MediaInfo } from '../model/media-info';
import { blobToImage } from '../helpers/blob-image.helper';
import DownloadIcon from '@mui/icons-material/Download';
import DeleteIcon from '@mui/icons-material/Delete';
import FavoriteBorderIcon from '@mui/icons-material/FavoriteBorder';
import { useEffect, useState } from 'react';

const backendUrl = process.env.REACT_APP_BACKEND_URL;

type Props = {
  media: MediaInfo;
};
export const MediaPreview = ({ media }: Props) => {
  const mediaDate = dateFromUnixTime(media.dateTimeOriginal);
  const country = media.country ?? '';
  const city = media.locality ?? media.region ?? '';
  const venue = media.venue ?? '';

  const address = city && country ? `${city}, ${country}${venue}` : '';

  const [isFavorite, setIsFavorite] = useState(media.isFavorite);

  useEffect(() => {
    // Download media from `${backendUrl}/media` endpoint
  }, [media]);

  const handleFavoriteClick = () => {
    media.isFavorite = !media.isFavorite;
    setIsFavorite(media.isFavorite);
    // TODO: Call API
  };

  return (
    <Card key={`media-preview-card${media.id}`}>
      <CardHeader subheader={media.fullPath} />
      <CardMedia
        key={`card-media-${media.id}`}
        component="img"
        width={media.thumbnailWidth * 3}
        height={media.thumbnailHeight * 3}
        image={blobToImage(media.thumbnail)}
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

      <CardActions disableSpacing sx={{ paddingTop: '1px', paddingBottom: '1px', display: 'flex' }}>
        <Box sx={{ flexGrow: 1 }}>
          <IconButton aria-label="Add to favorites" size="small" onClick={handleFavoriteClick}>
            {isFavorite ? <FavoriteIcon /> : <FavoriteBorderIcon />}
          </IconButton>
          <IconButton aria-label="Download" size="small">
            <DownloadIcon />
          </IconButton>
        </Box>
        <Box sx={{ flexGrow: 0 }}>
          <IconButton aria-label="Delete" size="small">
            <DeleteIcon />
          </IconButton>
        </Box>
      </CardActions>
    </Card>
  );
};
