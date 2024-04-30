import * as React from 'react';
import { Box, Card, CardContent, CardMedia, Checkbox, Typography } from '@mui/material';
import PlayCircleIcon from '@mui/icons-material/PlayCircle';
import { dateFromUnixTime, secondsToTimeFormat } from '../helpers/date-helper';
import { MediaInfo, MediaType } from '../model/media-info';
import { blobToImage } from '../helpers/blob-image.helper';
import RadioButtonUncheckedIcon from '@mui/icons-material/RadioButtonUnchecked';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import { pink } from '@mui/material/colors';

type Props = {
  media: MediaInfo;
  onClick: (media: MediaInfo) => void;
  showSelect?: boolean;
  onSelect?: (mediaId: string) => void;
};
export const MediaCard = ({ media, onClick, showSelect, onSelect }: Props) => {
  const mediaDate = dateFromUnixTime(media.dateTimeOriginal);
  const videoDuration: string = media.videoDurationSec
    ? secondsToTimeFormat(media.videoDurationSec)
    : '';
  // #004d40

  const onCheckBoxSelected = () => {
    if (onSelect) {
      onSelect(media.id);
    }
  };

  return (
    <Card key={`card-${media.id}`}>
      <Box sx={{ position: 'relative' }}>
        <CardMedia
          key={`card-media-${media.id}`}
          component="img"
          width={media.thumbnailWidth}
          height={media.thumbnailHeight}
          image={blobToImage(media.thumbnail)}
          onClick={() => onClick(media)}
        ></CardMedia>
        {media.mediaType === MediaType.VIDEO && (
          <Box
            sx={{
              display: 'flex',
              alignItems: 'center',
              position: 'absolute',
              top: '10px',
              left: '10px',
              gap: '5px',
            }}
          >
            <Typography
              id={`typography-0-${media.id}`}
              sx={{
                fontSize: '10px',
                color: 'white',
                display: 'flex',
                alignItems: 'center',
              }}
            >
              <PlayCircleIcon />
            </Typography>
            <Typography id={`typography-video-duration-${media.id}`} variant="body2" align="right">
              {videoDuration}
            </Typography>
          </Box>
        )}
        {showSelect && (
          <Box
            sx={{
              display: 'flex',
              alignItems: 'center',
              position: 'absolute',
              top: '5px',
              right: '5px',
            }}
          >
            <Checkbox
              aria-label="select"
              icon={<RadioButtonUncheckedIcon sx={{ color: pink[500] }} />}
              checkedIcon={<CheckCircleIcon sx={{ color: pink[500] }} />}
              onClick={onCheckBoxSelected}
            />
          </Box>
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
