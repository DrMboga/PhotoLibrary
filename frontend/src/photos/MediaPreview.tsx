import * as React from 'react';
import { useEffect, useState } from 'react';
import {
  Alert,
  Box,
  Card,
  CardActions,
  CardContent,
  CardHeader,
  CardMedia,
  IconButton,
  Typography,
} from '@mui/material';
import { dateFromUnixTime } from '../helpers/date-helper';
import FavoriteIcon from '@mui/icons-material/Favorite';
import { MediaInfo, MediaType } from '../model/media-info';
import { blobToImage } from '../helpers/blob-image.helper';
import DownloadIcon from '@mui/icons-material/Download';
import DeleteIcon from '@mui/icons-material/Delete';
import FavoriteBorderIcon from '@mui/icons-material/FavoriteBorder';
import { useAppSelector } from '../storeHooks';
import { selectToken } from '../keycloak-auth/authSlice';
import { backendAPI } from '../api/BackendApi';
import CircularProgress from '@mui/material/CircularProgress';

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

  const authToken = useAppSelector(selectToken);
  const [mediaData, setMediaData] = useState<Blob | undefined>(undefined);
  const [mediaDataAsUint8, setMediaDataAsUint8] = useState<Uint8Array>(new Uint8Array());
  const [mediaLoading, setMediaLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | undefined>(undefined);

  useEffect(() => {
    setMediaLoading(true);
    backendAPI
      .downloadMedia(media.fullPath, authToken)
      .then((value) => {
        setMediaData(value);
        value.arrayBuffer().then((valueAsArray) => {
          setMediaLoading(false);
          setMediaDataAsUint8(new Uint8Array(valueAsArray));
        });
      })
      .catch((err) => setError(err));
  }, [media, authToken]);

  const handleFavoriteClick = () => {
    media.isFavorite = !media.isFavorite;
    setIsFavorite(media.isFavorite);
    // TODO: Call API
  };

  const handleDownload = () => {
    if (mediaData) {
      const mediaUrl = window.URL.createObjectURL(mediaData);
      const tempLink = document.createElement('a');
      tempLink.href = mediaUrl;
      tempLink.setAttribute('download', media.fileName);
      tempLink.click();
    }
  };

  return (
    <Card key={`media-preview-card${media.id}`}>
      <CardHeader subheader={media.fileName} />
      {error && <Alert severity="error">{error}</Alert>}
      {mediaLoading && <CircularProgress />}
      {!mediaLoading && media.mediaType === MediaType.IMAGE && (
        <img
          src={blobToImage(mediaDataAsUint8)}
          alt={media.fileName}
          style={{ height: media.thumbnailHeight * 2 }}
        />
      )}
      {!mediaLoading && media.mediaType === MediaType.VIDEO && mediaData && (
        <video
          controls
          style={{ height: media.thumbnailHeight === 0 ? 504 : media.thumbnailHeight * 2 }}
        >
          <source src={URL.createObjectURL(mediaData)} />
        </video>
      )}
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
          <IconButton aria-label="Download" size="small" onClick={handleDownload}>
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
